using System.Collections;
using UnityEngine;

namespace Key.Scripts.Feedback {
    public class GunRecoil2D : MonoBehaviour {
        [Header("Target")] [SerializeField] private Transform recoilTarget;

        [Header("Recoil")] [SerializeField] private Vector2 recoilDirection = Vector2.left;
        [SerializeField, Min(0f)] private float recoilDistance = 0.15f;
        [SerializeField, Min(0.01f)] private float recoilDuration = 0.04f;
        [SerializeField, Min(0.01f)] private float recoveryDuration = 0.08f;

        private Vector3 _originLocalPosition;
        private Coroutine _recoilCoroutine;

        private void Awake() {
            if (recoilTarget == null)
                recoilTarget = transform;

            _originLocalPosition =
                recoilTarget.localPosition;
        }

        public void PlayRecoil() {
            if (recoilTarget == null)
                return;

            if (_recoilCoroutine != null)
                StopCoroutine(_recoilCoroutine);

            _recoilCoroutine =
                StartCoroutine(RecoilCoroutine());
        }

        private IEnumerator RecoilCoroutine() {
            Vector3 startPosition =
                recoilTarget.localPosition;

            Vector3 direction =
                ((Vector3)recoilDirection).normalized;

            Vector3 recoilPosition =
                _originLocalPosition +
                direction * recoilDistance;

            float elapsedTime = 0f;

            while (elapsedTime < recoilDuration) {
                elapsedTime += Time.deltaTime;

                float ratio = Mathf.Clamp01(
                    elapsedTime / recoilDuration
                );

                recoilTarget.localPosition =
                    Vector3.Lerp(
                        startPosition,
                        recoilPosition,
                        ratio
                    );

                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < recoveryDuration) {
                elapsedTime += Time.deltaTime;

                float ratio = Mathf.Clamp01(
                    elapsedTime / recoveryDuration
                );

                recoilTarget.localPosition =
                    Vector3.Lerp(
                        recoilPosition,
                        _originLocalPosition,
                        ratio
                    );

                yield return null;
            }

            recoilTarget.localPosition =
                _originLocalPosition;

            _recoilCoroutine = null;
        }

        private void OnDisable() {
            if (recoilTarget != null)
                recoilTarget.localPosition =
                    _originLocalPosition;

            _recoilCoroutine = null;
        }
    }
}