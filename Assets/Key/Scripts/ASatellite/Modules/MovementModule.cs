using UnityEngine;

namespace Key.Scripts.ASatellite.Modules {
    public class MovementModule : MonoBehaviour, IModule {
        [Header("Orbit")] [SerializeField] private float radius = 3f;
        [SerializeField] private float angularSpeed = 90f;
        [SerializeField] private float startAngle;

        private AbstractASatellite _owner;
        private Transform _center;

        private float _angle;
        private bool _isActive;

        public void Initialize(ModuleOwner owner) {
            _owner = owner as AbstractASatellite;

            if (_owner == null) {
                Debug.LogError(
                    $"{name}: ModuleOwner가 AbstractASatellite이 아닙니다.",
                    this
                );

                return;
            }
            _angle = startAngle;
            
            _owner.OnTick += Tick;
        }

        public void SetCenter(Transform center) {
            _center = center;
            _isActive = _center != null;

            if (!_isActive || _owner == null)
                return;

            SetPosition();
        }

        private void Tick(float deltaTime) {
            if (!_isActive || _center == null || _owner == null)
                return;

            _angle += angularSpeed * deltaTime;
            _angle %= 360f;

            SetPosition();
        }

        private void SetPosition() {
            float radian = _angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radian) * radius,
                Mathf.Sin(radian) * radius,
                0f
            );

            _owner.transform.position =
                _center.position + offset;
        }

        public void SetRadius(float value) {
            radius = Mathf.Max(0f, value);

            if (_isActive) {
                SetPosition();
            }
        }

        public void SetAngularSpeed(float value) {
            angularSpeed = value;
        }

        private void OnDestroy() {
            if (_owner != null) {
                _owner.OnTick -= Tick;
            }
        }
    }
}