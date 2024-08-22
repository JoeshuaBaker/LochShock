using UnityEngine;
using System.Collections.Generic;

namespace BulletHell
{
    public enum FollowTargetType
    {
        Homing,
        LockOnShot
    };

    public class ProjectileEmitterAdvanced : ProjectileEmitterBase
    {
        ColorPulse StaticOutlinePulse;
        ColorPulse StaticPulse;

        [Foldout("Appearance", true)]
        [SerializeField] public bool UseColorPulse;
        [ConditionalField(nameof(UseColorPulse)), SerializeField] protected float PulseSpeed;
        [ConditionalField(nameof(UseColorPulse)), SerializeField] protected bool UseStaticPulse;

        //[Foldout("Spokes", true)]
        protected int GroupCount {
            get
            {
                return Mathf.Max((int)stats.GetCombinedStatValue<BulletStreams>(World.activeWorld.worldStaticContext), 1);
            }
        }
        [Range(0, 1), SerializeField] protected float GroupSpacing = 1;
        [Range(0, 90), SerializeField] protected static float AccuracyAngle = 30;
        protected int SpokeCount {
            get
            {
                return Mathf.Max((int)stats.GetCombinedStatValue<BulletsPerShot>(World.activeWorld.worldStaticContext), 1);
            }
        }
        protected float SpokeSpacing {
            get
            {
                return stats.GetCombinedStatValue<SpreadAngle>(World.activeWorld.worldStaticContext);
            }
        }
        [SerializeField] protected bool MirrorPairRotation;
        [ConditionalField(nameof(MirrorPairRotation)), SerializeField] protected bool PairGroupDirection;

        [Foldout("Modifiers", true)]
        [SerializeField] public bool UseFollowTarget;
        [ConditionalField(nameof(UseFollowTarget))] public Transform Target;
        [ConditionalField(nameof(UseFollowTarget))] public FollowTargetType FollowTargetType = FollowTargetType.Homing;
        [ConditionalField(nameof(UseFollowTarget)), Range(0, 5)] public float FollowIntensity;

        [Foldout("Outline", true)]
        [SerializeField] protected bool UseOutlineColorPulse;
        [ConditionalField(nameof(UseOutlineColorPulse)), SerializeField] protected float OutlinePulseSpeed;
        [ConditionalField(nameof(UseOutlineColorPulse)), SerializeField] protected bool UseOutlineStaticPulse;

        protected EmitterGroup[] Groups;
        protected int LastGroupCountPoll = -1;
        protected int emitterId = -1;
        protected bool PreviousMirrorPairRotation = false;
        protected bool PreviousPairGroupDirection = false;

        public Dictionary<int, TrailRenderer> rendererDict;

        public new void Awake()
        {
            base.Awake();

            rendererDict = new Dictionary<int, TrailRenderer>();
            Groups = new EmitterGroup[10];
            RefreshGroups();
        }



        public virtual void Start()
        {
            ProjectileManager.AddEmitter(this, ProjectilePrefab.GetProjectilesPerEmitterInstance());
            emitterId = ProjectileManager.RegisterEmitter(this);
        }

        protected void RefreshGroups()
        {
            int tempGroups = 1;
            int tempSpokes = 1;
            float tempSpacing = 0f;

            System.Func<int> groupLookup = () => { return World.activeWorld == null ? tempGroups : GroupCount; };
            System.Func<int> spokesLookup = () => { return World.activeWorld == null ? tempSpokes : SpokeCount; };
            System.Func<float> spacingLookup = () => { return World.activeWorld == null ? tempSpacing : SpokeSpacing; };

            if (groupLookup.Invoke() > 10)
            {
                Debug.Log("Max Group Count is set to 10.  You attempted to set it to " + GroupCount.ToString() + ".");
                return;
            }

            bool mirror = false;
            if (Groups == null || LastGroupCountPoll != groupLookup.Invoke() || PreviousMirrorPairRotation != MirrorPairRotation || PreviousPairGroupDirection != PairGroupDirection)
            {
                // Refresh the groups, they were changed
                float rotation = 0;
                for (int n = 0; n < Groups.Length; n++)
                {
                    if (n < groupLookup.Invoke() && Groups[n] == null)
                    {
                        Groups[n] = new EmitterGroup(Rotate(Direction, rotation).normalized, spokesLookup.Invoke(), spacingLookup.Invoke(), mirror);
                    }
                    else if (n < groupLookup.Invoke())
                    {
                        Groups[n].Set(Rotate(Direction, rotation).normalized, spokesLookup.Invoke(), spacingLookup.Invoke(), mirror);
                    }
                    else
                    {
                        //n is greater than GroupCount -- ensure we clear the rest of the buffer
                        Groups[n] = null;
                    }

                    // invert the mirror flag if needed
                    if (MirrorPairRotation)
                        mirror = !mirror;

                    // sets the starting direction of all the groups so we divide by 360 to evenly distribute their direction
                    // Could reduce the scope of the directions here
                    rotation = CalculateGroupRotation(n, rotation, spacingLookup.Invoke(), groupLookup.Invoke());
                }
                LastGroupCountPoll = groupLookup.Invoke();
                PreviousMirrorPairRotation = MirrorPairRotation;
                PreviousPairGroupDirection = PairGroupDirection;
            }
            else if (RotationSpeed == 0)
            {
                float rotation = 0;
                // If rotation speed is locked, then allow to update Direction of groups
                for (int n = 0; n < Groups.Length; n++)
                {
                    if (Groups[n] != null)
                    {
                        Groups[n].Direction = Rotate(Direction, rotation).normalized;
                    }

                    rotation = CalculateGroupRotation(n, rotation, spacingLookup.Invoke(), groupLookup.Invoke());
                }
            }
        }

        public static Vector2 GetDirectionByStreamsAndShots(Vector2 lookDirection, int stream, int bulletStreams, int bullet, int bulletsPerShot, float accuracy, float spreadAngle)
        {
            Vector2 streamDirection = Rotate(lookDirection, 360f * (stream / (float)bulletStreams));
            streamDirection = Rotate(streamDirection, AccuracyAngle * Random.Range(-1f, 1f) * Mathf.Clamp(1f - accuracy, 0f, 1f));

            Vector2 bulletDirection;
            bool isEven = bulletsPerShot % 2 == 0;

            //Since bullet 0 always goes directly in the direction of the stream, we can handle 
            int indexBullet = bullet;
            int evenBullets = isEven ? bulletsPerShot : bulletsPerShot - 1;
                
            //skip over the middle angle once we're halfway through the bullets for even numbers of bullets
            if (isEven && indexBullet >= evenBullets / 2) 
                indexBullet += 1;
                
            float spreadAngleFraction = (spreadAngle*2) / evenBullets;
            float rotateDegrees = spreadAngle - spreadAngleFraction * indexBullet;
            
            //for even numbers of streams, bunch them a bit closer to 0 so not having a middle bullet stream isn't so annoying
            if (isEven)
            {
                rotateDegrees = rotateDegrees > 0 ? rotateDegrees + (spreadAngleFraction / 2) : rotateDegrees - (spreadAngleFraction / 2);
            }

            bulletDirection = Rotate(streamDirection, rotateDegrees);

            return bulletDirection;
        }

        public override void FireProjectile(Vector2 direction, float leakedTime)
        {
            Pool<ProjectileData>.Node node;

            Direction = direction;
            RefreshGroups();

            for (int stream = 0; stream < GroupCount; stream++)
            {
                if (Projectiles.AvailableCount >= SpokeCount)
                {
                    for (int bullet = 0; bullet < SpokeCount; bullet++)
                    {
                        float accuracy = stats.GetCombinedStatValue<Accuracy>(World.activeWorld.worldStaticContext);
                        Vector2 bulletDirection = GetDirectionByStreamsAndShots(Direction, stream, GroupCount, bullet, SpokeCount, accuracy, SpokeSpacing);
                        node = SetupBullet(Groups[stream], bulletDirection);

                        // Keep track of active projectiles                       
                        PreviousActiveProjectileIndexes[ActiveProjectileIndexesPosition] = node.NodeIndex;
                        ActiveProjectileIndexesPosition++;
                        if (ActiveProjectileIndexesPosition < ActiveProjectileIndexes.Length)
                        {
                            PreviousActiveProjectileIndexes[ActiveProjectileIndexesPosition] = -1;
                        }
                        else
                        {
                            Debug.Log("Error: Projectile was fired before list of active projectiles was refreshed.");
                        }

                        UpdateProjectile(ref node, leakedTime);

                        if (TrailTest.activeTrails != null && !rendererDict.ContainsKey(node.NodeIndex))
                        {
                            TrailTest trailTest = TrailTest.activeTrails;
                            TrailRenderer renderer = trailTest.GetRenderer();
                            renderer.transform.position = node.Item.Position;
                            renderer.startWidth = node.Item.size;
                            if(node.Item.size > 1f)
                            {
                                renderer.time = trailTest.trailPrefab.time + node.Item.size * 0.01f;
                            }
                            renderer.endWidth = 0;
                            renderer.gameObject.SetActive(true);
                            renderer.Clear();
                            rendererDict.Add(node.NodeIndex, renderer);
                        }
                    }

                    if (Groups[stream].InvertRotation)
                        Groups[stream].Direction = Rotate(Groups[stream].Direction, -RotationSpeed);
                    else
                        Groups[stream].Direction = Rotate(Groups[stream].Direction, RotationSpeed);
                }
            }
        }

        public virtual Pool<ProjectileData>.Node SetupBullet(EmitterGroup group, Vector2 direction)
        {
            Pool<ProjectileData>.Node node = Projectiles.Get();
            node.Item.SetupProjectileData(stats, transform.position, Speed * direction);

            node.Item.Gravity = Gravity;
            if (UseFollowTarget && FollowTargetType == FollowTargetType.LockOnShot && Target != null)
            {
                group.Direction = (Target.transform.position - transform.position).normalized;
            }
            node.Item.Color = Color.Evaluate(0);
            node.Item.Acceleration = Acceleration;
            node.Item.FollowTarget = UseFollowTarget;
            node.Item.FollowIntensity = FollowIntensity;
            node.Item.Target = Target;

            // Setup outline if we have one
            if (ProjectilePrefab.Outline != null && DrawOutlines)
            {
                Pool<ProjectileData>.Node outlineNode = ProjectileOutlines.Get();

                outlineNode.Item.Position = node.Item.Position;
                outlineNode.Item.size = node.Item.size + OutlineSize;
                outlineNode.Item.Color = OutlineColor.Evaluate(0);

                node.Item.Outline = outlineNode;
            }

            return node;
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Scale);

            Gizmos.color = UnityEngine.Color.yellow;

            float rotation = 0;

            for (int n = 0; n < GroupCount; n++)
            {
                Vector2 direction = Rotate(Direction, rotation).normalized * (Scale + 0.2f);
                Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + direction);

                rotation = CalculateGroupRotation(n, rotation, GroupSpacing, GroupCount);
            }

            Gizmos.color = UnityEngine.Color.red;
            rotation = 0;
            float spokeRotation = 0;
            bool left = true;
            for (int n = 0; n < GroupCount; n++)
            {
                Vector2 groupDirection = Rotate(Direction, rotation).normalized;
                spokeRotation = 0;
                left = true;

                for (int m = 0; m < SpokeCount; m++)
                {
                    Vector2 direction = Vector2.zero;
                    if (left)
                    {
                        direction = Rotate(groupDirection, spokeRotation).normalized * (Scale + 0.15f);
                        spokeRotation += SpokeSpacing;
                    }
                    else
                    {
                        direction = Rotate(groupDirection, -spokeRotation).normalized * (Scale + 0.15f);
                    }
                    Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + direction);

                    left = !left;
                }

                rotation = CalculateGroupRotation(n, rotation, GroupSpacing, GroupCount);
            }
        }

        private float CalculateGroupRotation(int index, float currentRotation, float groupSpacing, int groupCount)
        {
            if (PairGroupDirection)
            {
                if (index % 2 == 1)
                    currentRotation += 360 * groupSpacing * 2f / groupCount;
            }
            else
            {
                currentRotation += 360 * groupSpacing / groupCount;
            }
            return currentRotation;
        }

        protected override void UpdateProjectiles(float tick)
        {
            UpdateStaticPulses(tick);
            base.UpdateProjectiles(tick);
        }

        protected override void UpdateProjectile(ref Pool<ProjectileData>.Node node, float tick)
        {
            if (node.Active)
            {
                node.Item.lifetime -= tick;

                if (node.Item.lifetime > 0)
                {
                    UpdateProjectileVisuals(ref node, tick);

                    UpdateProjectileVelocity(ref node, tick);

                    ProcessCollision(ref node, tick);
                }
                else
                {
                    DestroyBullet(ref node);
                }

                if (TrailTest.activeTrails != null)
                {
                    TrailRenderer trailRenderer = null;
                    rendererDict.TryGetValue(node.NodeIndex, out trailRenderer);

                    if (trailRenderer != null)
                    {
                        if (node.Active)
                        {
                            trailRenderer.transform.position = node.Item.Position;
                        }
                        else
                        {
                            trailRenderer.transform.position = TrailTest.activeTrails.transform.position;
                            trailRenderer.Clear();
                            trailRenderer.gameObject.SetActive(false);
                            rendererDict.Remove(node.NodeIndex);
                        }
                    }
                }
            }
        }

        protected virtual void UpdateProjectileVisuals(ref Pool<ProjectileData>.Node node, float tick)
        {
            UpdateProjectileNodePulse(tick, ref node.Item);

            // If flag set - return projectiles that are no longer in view 
            if (CullProjectilesOutsideCameraBounds)
            {
                Bounds bounds = new Bounds(node.Item.Position, new Vector3(node.Item.size, node.Item.size, node.Item.size));
                if (!GeometryUtility.TestPlanesAABB(Planes, bounds))
                {
                    ReturnNode(ref node);
                    return;
                }
            }

            // Update foreground and outline color data
            UpdateProjectileColor(ref node.Item);
        }

        protected virtual void UpdateProjectileVelocity(ref Pool<ProjectileData>.Node node, float tick)
        {
            // apply acceleration
            node.Item.Velocity *= (1 + node.Item.Acceleration * tick);

            // follow target
            if (FollowTargetType == FollowTargetType.Homing && node.Item.FollowTarget && node.Item.Target != null)
            {
                node.Item.velocity += Acceleration * tick;

                Vector2 desiredVelocity = (new Vector2(Target.transform.position.x, Target.transform.position.y) - node.Item.Position).normalized;
                desiredVelocity *= node.Item.velocity;

                Vector2 steer = desiredVelocity - node.Item.Velocity;
                node.Item.Velocity = Vector2.ClampMagnitude(node.Item.Velocity + steer * node.Item.FollowIntensity * tick, node.Item.velocity);
            }
            else
            {
                // apply gravity
                node.Item.Velocity += node.Item.Gravity * tick;
            }
        }

        protected virtual int CheckCollision(ref Pool<ProjectileData>.Node node, float tick)
        {
            Vector2 deltaPosition = node.Item.DeltaPosition(tick);
            float distance = deltaPosition.magnitude;

            int result = -1;
            if (CollisionDetection == CollisionDetectionType.Raycast)
            {
                result = Physics2D.Raycast(node.Item.Position, deltaPosition, ContactFilter, RaycastHitBuffer, distance);
            }
            else if (CollisionDetection == CollisionDetectionType.CircleCast)
            {
                result = Physics2D.CircleCast(node.Item.Position, node.Item.Radius, deltaPosition, ContactFilter, RaycastHitBuffer, distance);
            }

            return result;
        }

        protected virtual void ProcessCollision(ref Pool<ProjectileData>.Node node, float tick)
        {
            int collisionResult = CheckCollision(ref node, tick);
            bool bounceMove = false;

            if (collisionResult > 0)
            {
                ProcessHit(ref node, tick);
                bounceMove = PhysicsMove(ref node, tick);
            }

            if(!bounceMove)
            {
                NonPhysicsMove(ref node, tick);
            }

        }

        // Put whatever hit code you want here such as damage events
        protected virtual void ProcessHit(ref Pool<ProjectileData>.Node node, float tick)
        {

        }

        protected virtual bool PhysicsMove(ref Pool<ProjectileData>.Node node, float tick)
        {
            bool bounceMove = false;

            // Collision was detected, should we bounce off or destroy the projectile?
            if (BounceOffSurfaces)
            {
                // Calculate the position the projectile is bouncing off the wall at
                Vector2 projectedNewPosition = node.Item.Position + (node.Item.DeltaPosition(tick) * RaycastHitBuffer[0].fraction);
                Vector2 directionOfHitFromCenter = RaycastHitBuffer[0].point - projectedNewPosition;
                float distanceToContact = (RaycastHitBuffer[0].point - projectedNewPosition).magnitude;
                float remainder = node.Item.Radius - distanceToContact;

                // reposition projectile to the point of impact 
                node.Item.Position = projectedNewPosition - (directionOfHitFromCenter.normalized * remainder);

                // reflect the velocity for a bounce effect -- will work well on static surfaces
                node.Item.Velocity = Vector2.Reflect(node.Item.Velocity, RaycastHitBuffer[0].normal);

                // calculate remaining distance after bounce
                node.Item.Position += node.Item.Velocity * tick * (1 - RaycastHitBuffer[0].fraction);

                // Absorbs energy from bounce
                node.Item.Velocity = new Vector2(node.Item.Velocity.x * (1 - BounceAbsorbtionX), node.Item.Velocity.y * (1 - BounceAbsorbtionY));
                bounceMove = true;
            }
            else
            {
                DestroyBullet(ref node);
            }

            return bounceMove;
        }

        protected virtual void NonPhysicsMove(ref Pool<ProjectileData>.Node node, float tick)
        {
            node.Item.Position += node.Item.DeltaPosition(tick);
        }

        protected virtual void DestroyBullet(ref Pool<ProjectileData>.Node node)
        {
            ReturnNode(ref node);
        }

        private void UpdateProjectileNodePulse(float tick, ref ProjectileData data)
        {
            if (UseColorPulse && !UseStaticPulse)
            {
                data.Pulse.Update(tick, PulseSpeed);
            }

            if (UseOutlineColorPulse && !UseOutlineStaticPulse)
            {
                data.OutlinePulse.Update(tick, OutlinePulseSpeed);
            }
        }

        private void UpdateStaticPulses(float tick)
        {
            //projectile pulse
            if (UseColorPulse && UseStaticPulse)
            {
                StaticPulse.Update(tick, PulseSpeed);
            }

            //outline pulse
            if (UseOutlineColorPulse && UseOutlineStaticPulse)
            {
                StaticOutlinePulse.Update(tick, OutlinePulseSpeed);
            }
        }

        protected override void UpdateProjectileColor(ref ProjectileData data)
        {
            // foreground
            if (UseColorPulse)
            {
                if (UseStaticPulse)
                {
                    data.Color = Color.Evaluate(StaticPulse.Fraction);
                }
                else
                {
                    data.Color = Color.Evaluate(data.Pulse.Fraction);
                }
            }
            else
            {
                data.Color = Color.Evaluate(1 - data.lifetime / TimeToLive);
            }

            //outline
            if (data.Outline.Item != null)
            {
                if (UseOutlineColorPulse)
                {
                    if (UseOutlineStaticPulse)
                    {
                        data.Outline.Item.Color = OutlineColor.Evaluate(StaticOutlinePulse.Fraction);
                    }
                    else
                    {
                        data.Outline.Item.Color = OutlineColor.Evaluate(data.OutlinePulse.Fraction);
                    }
                }
                else
                {
                    data.Outline.Item.Color = OutlineColor.Evaluate(1 - data.lifetime / TimeToLive);
                }
            }
        }

    }
}