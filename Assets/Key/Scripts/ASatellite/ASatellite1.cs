using Key.Scripts.ASatellite.Modules;
using UnityEngine;

namespace Key.Scripts.ASatellite {
    public class ASatellite1 : AbstractASatellite {
        [Header("Orbit")] [SerializeField] private Transform orbitCenter;

        private MovementModule _movementModule;

        protected override void Awake() {
            // ModuleOwner에서 모듈을 먼저 수집하고 초기화
            base.Awake();

            _movementModule = GetModule<MovementModule>();

            if (_movementModule == null) {
                Debug.LogError(
                    $"{name}에 MovementModule이 없습니다.",
                    this
                );
            }
        }

        private void Start() {
            if (orbitCenter != null) {
                _movementModule?.SetCenter(orbitCenter);
            }
        }

        public void Deploy(Transform center) {
            if (center == null) {
                Debug.LogError(
                    $"{name}: 회전 중심이 전달되지 않았습니다.",
                    this
                );

                return;
            }

            orbitCenter = center;
            _movementModule?.SetCenter(center);
        }

        public void StopMovement() {
            _movementModule?.SetCenter(null);
        }
    }
}