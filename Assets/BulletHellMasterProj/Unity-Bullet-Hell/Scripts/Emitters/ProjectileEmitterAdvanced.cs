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
                return (int) stats.gunStats.bulletsPerShot;
            }
        }
        [Range(0, 1), SerializeField] protected float GroupSpacing = 1;
        [Range(1, 10), SerializeField] protected int SpokeCount = 3;
        [Range(0, 100), SerializeField] protected float SpokeSpacing = 25;
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
        protected bool PreviousMirrorPairRotation = false;
        protected bool PreviousPairGroupDirection = false;

        public new void Awake()
        {
            base.Awake();

            Groups = new EmitterGroup[10];
            RefreshGroups();
        }

        void Start()
        {
            // To allow for the enable / disable checkbox in Inspector
        }

        protected void RefreshGroups()
        {
            if (GroupCount > 10)
            {
                Debug.Log("Max Group Count is set to 10.  You attempted to set it to " + GroupCount.ToString() + ".");
                return;
            }

            bool mirror = false;
            if (Groups == null || LastGroupCountPoll != GroupCount || PreviousMirrorPairRotation != MirrorPairRotation || PreviousPairGroupDirection != PairGroupDirection)
            {
                // Refresh the groups, they were changed
                float rotation = 0;
                for (int n = 0; n < Groups.Length; n++)
                {
                    if (n < GroupCount && Groups[n] == null)
                    {
                        Groups[n] = new EmitterGroup(Rotate(Direction, rotation).normalized, SpokeCount, SpokeSpacing, mirror);
                    }
                    else if (n < GroupCount)
                    {
                        Groups[n].Set(Rotate(Direction, rotation).normalized, SpokeCount, SpokeSpacing, mirror);
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
                    rotation = CalculateGroupRotation(n, rotation);
                }
                LastGroupCountPoll = GroupCount;
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

                    rotation = CalculateGroupRotation(n, rotation);
                }
            }
        }

        public override Pool<ProjectileData>.Node FireProjectile(Vector2 direction, float leakedTime)
        {
            Pool<ProjectileData>.Node node = Projectiles.Get();

            Direction = direction;
            RefreshGroups();

            if (!AutoFire)
            {
                if (Interval > 0) return node;
                else Interval = CoolOffTime;
            }

            List<int> fs = new List<int>();
            List<float> rotations = new List<float>();

            for (int g = 0; g < GroupCount; g++)
            {
                if (Projectiles.AvailableCount >= SpokeCount)
                {
                    float rotation = 0;
                    bool swap = false;

                    for (int n = 0; n < SpokeCount; n++)
                    {
                        var group = Groups[g];
                        node = SetupBullet(Groups[g]);
                        float spacing = SpokeSpacing;
                        int f;

                        if (SpokeCount % 2 == 0)
                        {
                            f = (n + 2) / 2;
                        }
                        else
                        {
                            f = (n+1) / 2;
                        }

                        if (swap)
                        {
                            f *= -1;
                        }

                        rotation = spacing * f;

                        if(SpokeCount % 2 == 0)
                        {
                            rotation -= SpokeSpacing / 2f * ((swap) ? -1 : 1);
                        }


                        rotations.Add(rotation);
                        fs.Add(f);
                        node.Item.Velocity = Speed * Rotate(group.Direction, rotation).normalized;

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

                        swap = !swap;
                    }

                    if (Groups[g].InvertRotation)
                        Groups[g].Direction = Rotate(Groups[g].Direction, -RotationSpeed);
                    else
                        Groups[g].Direction = Rotate(Groups[g].Direction, RotationSpeed);
                }
            }

            string rotDebug = "Rotations: ";
            string fDebug = "fs: ";

            foreach(var rotation in rotations)
            {
                rotDebug += rotation + ", ";
            }
            foreach(var f in fs)
            {
                fDebug += f + ", ";
            }

            Debug.Log(rotDebug);
            Debug.Log(fDebug);

            return node;
        }

        public virtual Pool<ProjectileData>.Node SetupBullet(EmitterGroup group)
        {
            Pool<ProjectileData>.Node node = Projectiles.Get();
            node.Item.Position = transform.position;
            node.Item.ApplyStatBlock(stats.gunStats);
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
                outlineNode.Item.stats.size = node.Item.stats.size+ OutlineSize;
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

                rotation = CalculateGroupRotation(n, rotation);
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

                rotation = CalculateGroupRotation(n, rotation);
            }
        }

        private float CalculateGroupRotation(int index, float currentRotation)
        {
            if (PairGroupDirection)
            {
                if (index % 2 == 1)
                    currentRotation += 360 * GroupSpacing * 2f / GroupCount;
            }
            else
            {
                currentRotation += 360 * GroupSpacing / GroupCount;
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
                node.Item.stats.lifetime -= tick;

                if (node.Item.stats.lifetime > 0)
                {
                    UpdateProjectileVisuals(ref node, tick);

                    UpdateProjectileVelocity(ref node, tick);

                    ProcessCollision(ref node, tick);
                }
                else
                {
                    DestroyBullet(ref node);
                }
            }
        }

        protected virtual void UpdateProjectileVisuals(ref Pool<ProjectileData>.Node node, float tick)
        {
            UpdateProjectileNodePulse(tick, ref node.Item);

            // If flag set - return projectiles that are no longer in view 
            if (CullProjectilesOutsideCameraBounds)
            {
                Bounds bounds = new Bounds(node.Item.Position, new Vector3(node.Item.stats.size, node.Item.stats.size, node.Item.stats.size));
                if (!GeometryUtility.TestPlanesAABB(Planes, bounds))
                {
                    ReturnNode(node);
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
                node.Item.stats.speed += Acceleration * tick;

                Vector2 desiredVelocity = (new Vector2(Target.transform.position.x, Target.transform.position.y) - node.Item.Position).normalized;
                desiredVelocity *= node.Item.stats.speed;

                Vector2 steer = desiredVelocity - node.Item.Velocity;
                node.Item.Velocity = Vector2.ClampMagnitude(node.Item.Velocity + steer * node.Item.FollowIntensity * tick, node.Item.stats.speed);
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

            if (collisionResult > 0)
            {
                ProcessHit(ref node, tick);
                PhysicsMove(ref node, tick);
            }
            else
            {
                NonPhysicsMove(ref node, tick);
            }
        }

        // Put whatever hit code you want here such as damage events
        protected virtual void ProcessHit(ref Pool<ProjectileData>.Node node, float tick)
        {

        }

        protected virtual void PhysicsMove(ref Pool<ProjectileData>.Node node, float tick)
        {
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
            }
            else
            {
                DestroyBullet(ref node);
            }
        }

        protected virtual void NonPhysicsMove(ref Pool<ProjectileData>.Node node, float tick)
        {
            node.Item.Position += node.Item.DeltaPosition(tick);
        }

        protected virtual void DestroyBullet(ref Pool<ProjectileData>.Node node)
        {
            ReturnNode(node);
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
                data.Color = Color.Evaluate(1 - data.stats.lifetime / TimeToLive);
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
                    data.Outline.Item.Color = OutlineColor.Evaluate(1 - data.stats.lifetime / TimeToLive);
                }
            }
        }

    }
}