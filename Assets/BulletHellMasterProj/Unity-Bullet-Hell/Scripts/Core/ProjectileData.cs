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
        public float pierces = 0f;
        public float bounces = 0f;
        public float size = 0f;
        public float lifetime = 0f;
        public float velocity = 0f;
        public float damage = 0f;
        public float knockback = 0f;

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

        public void ApplyStatBlock(StatBlock stats)
        {
            pierces = stats.GetStatValue<Pierce>();
            bounces = stats.GetStatValue<Bounce>();
            size = stats.GetStatValue<Size>();
            lifetime = stats.GetStatValue<Lifetime>();
            velocity = stats.GetStatValue<Velocity>();
            damage = stats.GetStatValue<Damage>();
            knockback = stats.GetStatValue<Knockback>();
        }

        public Vector2 DeltaPosition(float tick)
        {
            return Velocity * tick;
        }
    }
}