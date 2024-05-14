using UnityEngine;

namespace BulletHell
{
    public class GunEmitter : ProjectileEmitterAdvanced
    {
        public new void Awake()
        {
            LayerMask = (1 << UnityEngine.LayerMask.NameToLayer("Enemy"));
            base.Awake();
            ContactFilter.useLayerMask = true;
        }

        void Start()
        {
            // To allow for the enable / disable checkbox in Inspector
        }

        public void ApplyStatBlock(StatBlock stats)
        {
            this.stats = stats;
        }

        protected override void ProcessHit(ref Pool<ProjectileData>.Node node, float tick)
        {
            foreach (var hit in RaycastHitBuffer)
            {
                BulletCollidable bulletCollidable = hit.transform.GetComponent<BulletCollidable>();
                if (bulletCollidable != null)
                {
                    bulletCollidable.ProcessCollision(node.Item);
                }
            }
        }
    }
}