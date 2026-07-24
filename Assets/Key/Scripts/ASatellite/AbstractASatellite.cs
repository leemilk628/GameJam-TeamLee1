using System;
using Key.Scripts.ASatellite.Modules;
using UnityEngine;

namespace Key.Scripts.ASatellite {
    public abstract class AbstractASatellite : ModuleOwner {
        public event Action<float> OnTick;

        [SerializeField] protected ASatelliteSO satelliteData;

        public int AttackPower => attackPower;
        public float AttackSpeed => attackSpeed;
        public float AttackInterval => attackInterval;
        public GameObject BulletPrefab => bullet;

        protected int attackPower;
        protected float attackSpeed;
        protected float attackInterval;
        protected GameObject bullet;
        
        protected override void Awake() {
            base.Awake();

            if (satelliteData == null) {
                return;
            }

            ApplySOData();
        }

        protected virtual void Update() {
            OnTick?.Invoke(Time.deltaTime);
        }

        protected virtual void ApplySOData() {
            attackPower = satelliteData.attackPower;
            attackSpeed = satelliteData.attackSpeed;
            bullet = satelliteData.bullet;

            RecalculateAttackInterval();
        }

        public virtual void Deploy(Transform orbitCenter) {
            if (orbitCenter == null) {
                return;
            }

            MovementModule movementModule =
                GetModule<MovementModule>();

            if (movementModule == null) {   
                return;
            }

            movementModule.SetCenter(orbitCenter);

            AttackModule attackModule =
                GetModule<AttackModule>();

            attackModule?.Activate();
        }

        public void Upgrade(int enforceLevel) {
            if (satelliteData == null)
                return;

            if (enforceLevel < 0 ||
                enforceLevel >= satelliteData.damageIncreaseAmount.Length ||
                enforceLevel >= satelliteData.attackSpeedIncreaseAmount.Length) {
                return;
            }

            float damageIncreaseRate =
                satelliteData.damageIncreaseAmount[enforceLevel];

            float attackSpeedIncreaseRate =
                satelliteData.attackSpeedIncreaseAmount[enforceLevel];

            attackPower = Mathf.RoundToInt(
                attackPower * (1f + damageIncreaseRate)
            );

            attackSpeed *= 1f + attackSpeedIncreaseRate;

            RecalculateAttackInterval();
        }

        private void RecalculateAttackInterval() {
            if (attackSpeed <= 0f) {
                attackInterval = float.PositiveInfinity;
                return;
            }

            attackInterval = 1f / attackSpeed;
        }
    }
}