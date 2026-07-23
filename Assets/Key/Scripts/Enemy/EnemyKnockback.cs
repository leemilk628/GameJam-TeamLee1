using System.Collections;
using UnityEngine;

namespace Key.Scripts.Enemy {
    
    //Enemy 이동 코드에 넉백 받을 때 멈추는 코드 쓸 것.
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyKnockback : MonoBehaviour, IKnockbackable {
        [Header("Knockback")]
        [SerializeField] private float knockbackDuration = 0.12f;
        [SerializeField] private float knockbackResistance = 1f;

        public bool IsKnockback { get; private set; }

        private Rigidbody2D _rigidbody;
        private Coroutine _knockbackCoroutine;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Knockback(Vector2 direction, float power) {
            if (direction == Vector2.zero || power <= 0f)
                return;

            if (_knockbackCoroutine != null)
                StopCoroutine(_knockbackCoroutine);

            _knockbackCoroutine = StartCoroutine(
                KnockbackCoroutine(direction.normalized, power)
            );
        }

        private IEnumerator KnockbackCoroutine(Vector2 direction, float power) {
            IsKnockback = true;

            float elapsedTime = 0f;
            float finalPower = power * knockbackResistance;

            while (elapsedTime < knockbackDuration) {
                float ratio = elapsedTime / knockbackDuration;
                float currentPower = Mathf.Lerp(finalPower, 0f, ratio);

                _rigidbody.linearVelocity = direction * currentPower;

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _rigidbody.linearVelocity = Vector2.zero;

            IsKnockback = false;
            _knockbackCoroutine = null;
        }

        private void OnDisable() {
            if (_knockbackCoroutine != null)
                StopCoroutine(_knockbackCoroutine);

            if (_rigidbody != null)
                _rigidbody.linearVelocity = Vector2.zero;

            IsKnockback = false;
            _knockbackCoroutine = null;
        }
    }
}