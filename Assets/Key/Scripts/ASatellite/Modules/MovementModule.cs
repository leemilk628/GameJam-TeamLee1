using UnityEngine;

namespace Key.Scripts.ASatellite.Modules {
    public class MovementModule : MonoBehaviour, IModule {
        [SerializeField] private float radius = 3f;
        [SerializeField] private float angularSpeed = 90f;
        [SerializeField] private float startAngle;

        private AbstractASatellite _owner;
        private Transform _center;

        private float _angle;
        private bool _isActive;

        public void Initialize(ModuleOwner owner) {
            _owner = owner as AbstractASatellite;

            if (_owner == null)
                return;

            _angle = startAngle;

            _owner.OnTick += Tick;
        }

        public void SetCenter(Transform center) {
            _center = center;
            _isActive = center != null;
        }

        private void Tick(float deltaTime) {
            if (!_isActive || _center == null)
                return;

            _angle += angularSpeed * deltaTime;

            float radian = _angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radian) * radius,
                Mathf.Sin(radian) * radius,
                0f
            );

            _owner.transform.position =
                _center.position + offset;
        }

        private void OnDestroy() {
            if (_owner != null) {
                _owner.OnTick -= Tick;
            }
        }
    }
}