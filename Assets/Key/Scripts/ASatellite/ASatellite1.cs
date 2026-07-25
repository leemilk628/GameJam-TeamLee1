using Key.Scripts.ASatellite.Modules;
using UnityEngine;

namespace Key.Scripts.ASatellite {
    public class ASatellite1 : AbstractASatellite {
        [Header("Orbit")]
        [SerializeField] private Transform orbitCenter;

        private MovementModule _movementModule;
        private AttackModule _attackModule;

        protected override void Awake() {
            base.Awake();

            _movementModule = GetModule<MovementModule>();
            _attackModule = GetModule<AttackModule>();
        }

        protected override void Start() {
            base.Start();

            if (orbitCenter != null) {
                Deploy(orbitCenter);
            }
        }

        public override void Deploy(Transform center) {
            if (center == null) {
                return;
            }

            orbitCenter = center;

            base.Deploy(center);
        }

        public void StopMovement() {
            _movementModule?.SetCenter(null);
        }

        public void StopAttack() {
            _attackModule?.Deactivate();
        }

        public void ResumeAttack() {
            _attackModule?.Activate();
        }
    }
}
