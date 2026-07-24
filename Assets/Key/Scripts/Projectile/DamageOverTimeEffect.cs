using System.Collections;
using UnityEngine;

namespace Key.Scripts.Projectile {
    public class DamageOverTimeEffect : MonoBehaviour {
        private MonoBehaviour _target;
        private int _damagePerTick;
        private float _duration;
        private float _tickInterval;

        public void Initialize(
            MonoBehaviour target,
            int damagePerTick,
            float duration,
            float tickInterval
        ) {
            _target = target;
            _damagePerTick = damagePerTick;
            _duration = duration;
            _tickInterval = tickInterval;

            StartCoroutine(ApplyDamage());
        }

        private IEnumerator ApplyDamage() {
            WaitForSeconds wait =
                new WaitForSeconds(_tickInterval);

            float elapsedTime = 0f;

            while (elapsedTime < _duration) {
                yield return wait;

                elapsedTime += _tickInterval;

                if (_target == null ||
                    !_target.gameObject.activeInHierarchy) {
                    break;
                }

                IDamageable damageable =
                    _target as IDamageable;

                if (damageable == null)
                    break;

                damageable.GetDamage(_damagePerTick);
            }

            Destroy(gameObject);
        }
    }
}