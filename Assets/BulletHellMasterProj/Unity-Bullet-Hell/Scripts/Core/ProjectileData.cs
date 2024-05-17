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
        public StatBlock.GunStats stats;
        public float Radius { 
            get
            {
                float radius = 0;
                if (Outline.Item != null)
                {
                    radius = Outline.Item.stats.size / 2f;
                }
                else
                {
                    radius = stats.size / 2f;
                }

                return radius;
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

        public void ApplyStatBlock(StatBlock.GunStats stats)
        {
            this.stats = stats;
        }

        public Vector2 DeltaPosition(float tick)
        {
            return Velocity * tick;
        }
    }
}