using UnityEngine;
using System.Collections.Generic;

namespace BulletHell
{
    public class GunEmitter : ProjectileEmitterAdvanced
    {
        public Gun gun;
        RaycastHit2D bounceTarget;
        public new void Awake()
        {
            LayerMask = (1 << UnityEngine.LayerMask.NameToLayer("Enemy"));
            base.Awake();
            ContactFilter.useLayerMask = true;
        }

        void Start()
        {

        }

        public void ApplyStatBlock(StatBlock stats)
        {
            this.stats = stats;
        }

        protected override void ProcessHit(ref Pool<ProjectileData>.Node node, float tick)
        {
            for (int i = 0; i < RaycastHitBuffer.Count; i++)
            {
                string hitName = RaycastHitBuffer[i].transform.name;
                if (node.Item.IgnoreList.Contains(hitName))
                {
                    var hitStruct = RaycastHitBuffer[i];
                    hitStruct.distance = -1;
                    RaycastHitBuffer[i] = hitStruct;
                    continue;
                }

                Enemy enemy = RaycastHitBuffer[i].transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    node.Item.IgnoreList.Add(hitName);
                    enemy.ProcessCollision(node.Item);
                    foreach(OnHitAction onHit in stats.events.OnHit)
                    {
                        onHit.OnHit(Player.activePlayer, gun, node.Item, enemy);
                    }

                    if(enemy.IsDead())
                    {
                        foreach(OnKillAction onKill in stats.events.OnKill)
                        {
                            onKill.OnKill(Player.activePlayer, gun, node.Item, enemy);
                        }
                    }
                }
                else
                {
                    BulletCollidable bulletCollidable = RaycastHitBuffer[i].transform.GetComponent<BulletCollidable>();
                    if(bulletCollidable != null)
                    {
                        bulletCollidable.ProcessCollision(node.Item);
                    }
                }

                if(i == 0 || (bounceTarget.distance == -1 && RaycastHitBuffer[i].distance > -1))
                {
                    bounceTarget = RaycastHitBuffer[i];
                }
            }
        }

        protected override void PhysicsMove(ref Pool<ProjectileData>.Node node, float tick)
        {
            if(node.Item.stats.pierce > 0)
            {
                foreach(var hit in RaycastHitBuffer)
                {
                    if(hit.distance > -1)
                    {
                        node.Item.stats.pierce--;

                        //clear ignorelist of pierce targets, except for this one.
                        if(node.Item.stats.pierce <= 0)
                        {
                            node.Item.IgnoreList.Clear();
                            node.Item.IgnoreList.Add(hit.transform.name);
                            break;
                        }
                    }
                }
            }
            else if (node.Item.stats.bounce > 0)
            {
                node.Item.stats.bounce--;
                // Calculate the position the projectile is bouncing off the wall at
                Vector2 projectedNewPosition = node.Item.Position + (node.Item.DeltaPosition(tick) * bounceTarget.fraction);
                Vector2 directionOfHitFromCenter = bounceTarget.point - projectedNewPosition;
                float distanceToContact = (bounceTarget.point - projectedNewPosition).magnitude;
                float remainder = node.Item.Radius - distanceToContact;

                // reposition projectile to the point of impact 
                node.Item.Position = projectedNewPosition - (directionOfHitFromCenter.normalized * remainder);

                // reflect the velocity for a bounce effect -- will work well on static surfaces
                node.Item.Velocity = Vector2.Reflect(node.Item.Velocity, bounceTarget.normal);

                // calculate remaining distance after bounce
                node.Item.Position += node.Item.Velocity * tick * (1 - bounceTarget.fraction);

                // Absorbs energy from bounce
                node.Item.Velocity = new Vector2(node.Item.Velocity.x * (1 - BounceAbsorbtionX), node.Item.Velocity.y * (1 - BounceAbsorbtionY));
            }
            else
            {
                DestroyBullet(ref node);
            }
        }

        protected override void NonPhysicsMove(ref Pool<ProjectileData>.Node node, float tick)
        {
            base.NonPhysicsMove(ref node, tick);

        }
    }
}