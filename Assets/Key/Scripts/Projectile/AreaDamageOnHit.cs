using System.Collections.Generic;
using Key.Scripts.Singletone;
using UnityEngine;

namespace Key.Scripts.Projectile {
    public class AreaDamageOnHit : MonoBehaviour {
        [Header("Area Damage")]
        [SerializeField, Min(0.1f)] private float damageRadius = 2f;
        [SerializeField] private LayerMask targetLayer;

        [Header("Particle")]
        [SerializeField] private ParticleSystem hitParticlePrefab;

        public void Explode(
            Vector2 explosionPosition,
            int damage,
            float knockbackPower
        ) {
            SoundManager.Instance?.PlaySFX(SoundType.Explosion);
            
            PlayHitParticle(explosionPosition);

            Collider2D[] targets =
                Physics2D.OverlapCircleAll(
                    explosionPosition,
                    damageRadius,
                    targetLayer
                );

            HashSet<IDamageable> damagedTargets = new();

            foreach (Collider2D target in targets) {
                if (target == null)
                    continue;

                IDamageable damageable =
                    target.GetComponentInParent<IDamageable>();

                if (damageable == null)
                    continue;

                if (!damagedTargets.Add(damageable))
                    continue;

                damageable.GetDamage(damage);

                IKnockbackable knockbackable =
                    target.GetComponentInParent<IKnockbackable>();

                if (knockbackable == null)
                    continue;

                Vector2 knockbackDirection = (
                    (Vector2)target.bounds.center -
                    explosionPosition
                ).normalized;

                knockbackable.Knockback(
                    knockbackDirection,
                    knockbackPower
                );
            }
        }

        private void PlayHitParticle(Vector2 position) {
            if (hitParticlePrefab == null)
                return;

            ParticleSystem particle = Instantiate(
                hitParticlePrefab,
                position,
                Quaternion.identity
            );

            particle.Play();

            ParticleSystem.MainModule main =
                particle.main;

            float destroyTime =
                main.duration +
                main.startLifetime.constantMax;

            Destroy(
                particle.gameObject,
                destroyTime
            );
        }
    }
}