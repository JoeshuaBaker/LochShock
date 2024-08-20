using UnityEngine;
using System.Collections.Generic;

namespace BulletHell
{   
    public class ProjectileData
    {
        public Vector2 Velocity;
        public float Acceleration;
        public Vector2 Gravity;
        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                if(Outline.Item != null)
                {
                    Outline.Item.position = value;
                }
            }
        }
        public float Rotation;
        public Color Color;
        public CombinedStatBlock stats;
        public float pierces = 0f;
        public float bounces = 0f;
        public float size = 0f;
        public float lifetime = 0f;
        public float velocity = 0f;

        public float Radius { 
            get
            {
                return size / 2f;
            }
        }

        public ColorPulse Pulse;
        public ColorPulse OutlinePulse;

        public Transform Target;
        public bool FollowTarget;
        public float FollowIntensity;

        public HashSet<string> IgnoreList;

        // Stores the pooled node that is used to draw the shadow for this projectile
        public Pool<ProjectileData>.Node Outline;
        public DamageContext bulletContext;

        public void ApplyStatBlock(CombinedStatBlock stats)
        {
            this.stats = stats;
            pierces = stats.GetCombinedStatValue<Pierce>(World.activeWorld.worldStaticContext);
            bounces = stats.GetCombinedStatValue<Bounce>(World.activeWorld.worldStaticContext);
            size = stats.GetCombinedStatValue<Size>(World.activeWorld.worldStaticContext);
            lifetime = stats.GetCombinedStatValue<Lifetime>(World.activeWorld.worldStaticContext);
            velocity = stats.GetCombinedStatValue<Velocity>(World.activeWorld.worldStaticContext);
        }

        public void SetupBulletContext(Gun source)
        {
            bulletContext = new DamageContext
            {
                damageType = source.damageType,
                source = source,
                hitEnemies = null,
                hitBoss = null
            };
        }

        public Vector2 DeltaPosition(float tick)
        {
            return Velocity * tick;
        }
    }
}