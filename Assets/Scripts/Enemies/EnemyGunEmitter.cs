using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell
{
    public class EnemyGunEmitter : GunEmitter
{
        public new void Awake()
        {
            base.Awake();
            LayerMask = (1 << UnityEngine.LayerMask.NameToLayer("Player"));
            Interval = 0.25f;      // Start with a delay to allow time for scene to load
            Camera = Camera.main;

            ContactFilter = new ContactFilter2D
            {
                layerMask = LayerMask,
                useTriggers = false,
                useLayerMask = true
            };

            ProjectileManager = ProjectileManager.Instance;

            // If projectile type is not set, use default
            if (ProjectilePrefab == null)
                ProjectilePrefab = ProjectileManager.Instance.GetProjectilePrefab(0);
        }

        protected override void ProcessHit(ref Pool<ProjectileData>.Node node, float tick)
        {
            for (int i = 0; i < RaycastHitBuffer.Count; i++)
            {
                BulletCollidable hitTarget = RaycastHitBuffer[i].transform.GetComponent<BulletCollidable>();
                if (hitTarget is Player)
                {
                    Player player = hitTarget as Player;
                    player.ProcessCollision(node.Item, RaycastHitBuffer[i]);
                }
                else
                {
                    Debug.LogWarning("Enemy bullet collided with non-player object: " + RaycastHitBuffer[i].transform.gameObject.name);
                }
            }
        }
    }
}