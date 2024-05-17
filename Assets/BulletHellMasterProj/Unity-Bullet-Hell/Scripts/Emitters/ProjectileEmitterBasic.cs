using UnityEngine;

namespace BulletHell
{
    // Most basic emitter implementation
    public class ProjectileEmitterBasic : ProjectileEmitterBase
    {
        public override void FireProjectile(Vector2 direction, float leakedTime)
        {
            Pool<ProjectileData>.Node node = Projectiles.Get();

            node.Item.Position = transform.position;
            node.Item.ApplyStatBlock(stats.gunStats);
            node.Item.stats.lifetime = TimeToLive - leakedTime;
            node.Item.Velocity = Speed * Direction.normalized;
            node.Item.Position += node.Item.Velocity * leakedTime;
            node.Item.Color = new Color(0.6f, 0.7f, 0.6f, 1);
            node.Item.Acceleration = Acceleration;

            Direction = Rotate(Direction, RotationSpeed);
        }

        protected override void UpdateProjectile(ref Pool<ProjectileData>.Node node, float tick)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateProjectiles(float tick)
        {
            throw new System.NotImplementedException();
        }
    }
}