using UnityEngine;

namespace Key.Scripts.Projectile {
    public class DamageOverTimeOnHit : MonoBehaviour {
        [Header("Damage Over Time")]
        [SerializeField, Min(0.1f)] private float _duration = 3f;
        [SerializeField, Min(0.1f)] private float _tickInterval = 1f;

        [Range(0f, 2f)]
        [SerializeField] private float _damageMultiplier = 0.3f;

        public void Apply(
            Collider2D targetCollider,
            int bulletDamage
        ) {
            if (targetCollider == null)
                return;

            IDamageable damageable =
                targetCollider.GetComponentInParent<IDamageable>();

            MonoBehaviour targetBehaviour =
                damageable as MonoBehaviour;

            if (targetBehaviour == null)
                return;

            int damagePerTick = Mathf.Max(
                1,
                Mathf.RoundToInt(
                    bulletDamage * _damageMultiplier
                )
            );

            GameObject effectObject =
                new GameObject("DamageOverTimeEffect");

            DamageOverTimeEffect effect =
                effectObject.AddComponent<DamageOverTimeEffect>();

            effect.Initialize(
                targetBehaviour,
                damagePerTick,
                _duration,
                _tickInterval
            );
        }
    }
}