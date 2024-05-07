using UnityEngine;

namespace BulletHell
{
    public class GunEmitter : ProjectileEmitterAdvanced
    {
        [Foldout("Item Properties", true)]
        public int Damage = 0;
        public int bouncesRemaining = 0;
        public int piercesRemaining = 0;
        public float Knockback = 0;

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
            SpokeCount = (int) stats.gunStats.bulletsPerShot;

            Damage = (int) stats.bulletStats.damage;
            Scale = stats.bulletStats.size;
            Knockback = stats.bulletStats.knockback;
            bouncesRemaining = (int) stats.bulletStats.bounce;
            piercesRemaining = (int) stats.bulletStats.pierce;
        }

        public override Pool<ProjectileData>.Node SetupBullet(EmitterGroup group, float rotation, bool left)
        {
            var node = base.SetupBullet(group, rotation, left);
            node.Item.Damage = Damage;
            node.Item.Knockback = Knockback;
            return node;
        }

        protected override void ProcessHit(ref Pool<ProjectileData>.Node node, float tick)
        {
            base.ProcessHit(ref node, tick);
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