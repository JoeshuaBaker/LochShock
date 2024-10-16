﻿using UnityEngine;
using System.Collections.Generic;

namespace BulletHell
{
    public class GunEmitter : ProjectileEmitterAdvanced
    {
        public Gun gun;
        RaycastHit2D bounceTarget;
        HashSet<int> ignoreList;
        public new void Awake()
        {
            LayerMask = (1 << UnityEngine.LayerMask.NameToLayer("Enemy"));
            base.Awake();
            ContactFilter.useLayerMask = true;
        }

        public override void Start()
        {
            ignoreList = new HashSet<int>();
            base.Start();
        }

        public void ApplyStatBlock(CombinedStatBlock stats)
        {
            this.stats = stats;
        }

        public override Pool<ProjectileData>.Node SetupBullet(EmitterGroup group, Vector2 direction)
        {
            var bullet = base.SetupBullet(group, direction);
            bullet.Item.SetupBulletContext(gun);
            bullet.Item.Position += direction * gun.firePositionOffset;
            return bullet;
        }

        protected override void ProcessHit(ref Pool<ProjectileData>.Node node, float tick)
        {
            ignoreList.Clear();
            bounceTarget.distance = int.MaxValue;

            if(node.Item.bulletContext.hitEnemies == null)
            {
                node.Item.bulletContext.hitEnemies = new HashSet<Enemy>();
            }
            else
            {
                node.Item.bulletContext.hitEnemies.Clear();
            }

            for (int i = 0; i < RaycastHitBuffer.Count; i++)
            {
                string hitName = RaycastHitBuffer[i].transform.name;
                if (node.Item.IgnoreList.Contains(hitName))
                {
                    ignoreList.Add(i);
                    continue;
                }

                BulletCollidable hitTarget = RaycastHitBuffer[i].transform.GetComponent<BulletCollidable>();
                if (hitTarget is Enemy)
                {
                    Enemy enemy = hitTarget as Enemy;
                    node.Item.IgnoreList.Add(hitName);
                    node.Item.bulletContext.hitEnemies.Add(enemy);
                    enemy.ProcessCollision(node.Item, RaycastHitBuffer[i]);

                    foreach (OnHitAction onHit in stats.combinedStatBlock.GetEvents<OnHitAction>())
                    {
                        Item source = Player.activePlayer.inventory.FindEventSource(onHit);
                        onHit.OnHit(gun, Player.activePlayer, gun, node.Item, enemy);
                    }

                    if(enemy.IsDead())
                    {
                        foreach(OnKillAction onKill in stats.combinedStatBlock.GetEvents<OnKillAction>())
                        {
                            Item source = Player.activePlayer.inventory.FindEventSource(onKill);
                            onKill.OnKill(source, Player.activePlayer, gun, node.Item, enemy);
                        }
                    }
                }
                else
                {
                    if(hitTarget != null)
                    {
                        hitTarget.ProcessCollision(node.Item, RaycastHitBuffer[i]);
                    }
                }

                if(RaycastHitBuffer[i].distance < bounceTarget.distance)
                {
                    bounceTarget = RaycastHitBuffer[i];
                }
            }
        }

        protected override bool PhysicsMove(ref Pool<ProjectileData>.Node node, float tick)
        {
            bool bounceMove = false;
            if(node.Item.pierces > 0)
            {
                for(int i = 0; i < RaycastHitBuffer.Count; i++)
                {
                    if (ignoreList.Contains(i))
                    {
                        continue;
                    }

                    var hit = RaycastHitBuffer[i];
                    node.Item.pierces--;
                    node.Item.bulletContext.numPierces += 1;

                    //clear ignorelist of pierce targets, except for this one.
                    if (node.Item.pierces <= 0)
                    {
                        node.Item.IgnoreList.Clear();
                        node.Item.IgnoreList.Add(hit.transform.name);
                        break;
                    }
                }
            }
            else if (node.Item.bounces > 0 && bounceTarget.distance != int.MaxValue)
            {
                node.Item.bounces--;
                node.Item.bulletContext.numBounces += 1;

                // Calculate the position the projectile is bouncing off the wall at
                Vector2 projectedNewPosition = node.Item.Position + (node.Item.DeltaPosition(tick) * bounceTarget.fraction);
                Vector2 directionOfHitFromCenter = bounceTarget.point - projectedNewPosition;
                float distanceToContact = (bounceTarget.point - projectedNewPosition).magnitude;
                float remainder = node.Item.Radius - distanceToContact;
                var lastPosition = node.Item.Position;

                // reposition projectile to the point of impact 
                node.Item.Position = projectedNewPosition - (directionOfHitFromCenter.normalized * remainder);

                // reflect the velocity for a bounce effect -- will work well on static surfaces
                node.Item.Velocity = Vector2.Reflect(node.Item.Velocity, bounceTarget.normal);

                // calculate remaining distance after bounce
                node.Item.Position += node.Item.Velocity * tick * (1 - bounceTarget.fraction);

                if ((node.Item.Position - lastPosition).magnitude > 3f)
                {
                    Debug.Log($"Bullet {node.NodeIndex} moved from {lastPosition} to {node.Item.Position}");
                }

                // Absorbs energy from bounce
                node.Item.Velocity = new Vector2(node.Item.Velocity.x * (1 - BounceAbsorbtionX), node.Item.Velocity.y * (1 - BounceAbsorbtionY));
                bounceMove = true;
            }
            else
            {
                for (int i = 0; i < RaycastHitBuffer.Count; i++)
                {
                    if(!ignoreList.Contains(i))
                    {
                        DestroyBullet(ref node);
                        break;
                    }
                }
            }

            return bounceMove;
        }

        protected override void NonPhysicsMove(ref Pool<ProjectileData>.Node node, float tick)
        {
            base.NonPhysicsMove(ref node, tick);
        }
    }
}