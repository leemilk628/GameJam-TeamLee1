using UnityEngine;

namespace Key.Scripts.Feedback {
    public class CameraShake2D : MonoBehaviour {
        [Header("Shake Power")]
        [SerializeField] private float maxPositionOffset = 0.08f;
        [SerializeField] private float maxRotationOffset = 0.7f;
        [SerializeField] private float frequency = 25f;
        [SerializeField] private float recoverySpeed = 3f;

        private Vector3 _startLocalPosition;
        private Quaternion _startLocalRotation;
        private float _trauma;

        private void Awake() {
            _startLocalPosition = transform.localPosition;
            _startLocalRotation = transform.localRotation;
        }

        private void LateUpdate() {
            _trauma = Mathf.MoveTowards(
                _trauma,
                0f,
                recoverySpeed * Time.unscaledDeltaTime
            );

            float shakePower = _trauma * _trauma;
            float noiseTime = Time.unscaledTime * frequency;

            float offsetX = GetNoise(noiseTime, 0f)
                * maxPositionOffset
                * shakePower;

            float offsetY = GetNoise(0f, noiseTime)
                * maxPositionOffset
                * shakePower;

            float rotation = GetNoise(noiseTime, noiseTime)
                * maxRotationOffset
                * shakePower;

            transform.localPosition = _startLocalPosition
                + new Vector3(offsetX, offsetY, 0f);

            transform.localRotation = _startLocalRotation
                * Quaternion.Euler(0f, 0f, rotation);
        }

        private float GetNoise(float x, float y) {
            return Mathf.PerlinNoise(x, y) * 2f - 1f;
        }

        public void AddShake(float amount) {
            if (amount <= 0f)
                return;

            _trauma = Mathf.Clamp01(_trauma + amount);
        }

        private void OnDisable() {
            transform.localPosition = _startLocalPosition;
            transform.localRotation = _startLocalRotation;
            _trauma = 0f;
        }
    }
}