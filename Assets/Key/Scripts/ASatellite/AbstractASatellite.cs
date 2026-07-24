using System;
using Eric.Satellite;
using Key.Scripts.ASatellite.Modules;
using UnityEngine;

namespace Key.Scripts.ASatellite {
    public abstract class AbstractASatellite : ModuleOwner {
        public event Action<float> OnTick;

        [SerializeField] protected ASatelliteSO satelliteData;

        public int AttackPower {
            get {
                return Mathf.Max(
                    0,
                    attackPower +
                    GetSharedAttackBonus()
                );
            }
        }

        public float AttackSpeed {
            get {
                return Mathf.Max(
                    0.01f,
                    attackSpeed +
                    GetSharedAttackSpeedBonus()
                );
            }
        }

        public float AttackInterval =>
            attackInterval;

        public GameObject BulletPrefab =>
            bullet;

        protected int attackPower;
        protected float attackSpeed;
        protected float attackInterval;
        protected GameObject bullet;

        private SatelliteStageStatModule _satelliteStatModule;

        protected override void Awake() {
            base.Awake();

            if (satelliteData == null) {
                Debug.LogError(
                    $"{name}: ASatelliteSO가 설정되지 않았습니다.",
                    this
                );

                return;
            }

            ApplySOData();
        }

        protected virtual void Start() {
            ConnectSatelliteStatModule();
            RecalculateAttackInterval();
        }

        protected virtual void Update() {
            if (_satelliteStatModule == null)
                ConnectSatelliteStatModule();

            RecalculateAttackInterval();

            OnTick?.Invoke(Time.deltaTime);
        }

        protected virtual void ApplySOData() {
            attackPower =
                satelliteData.attackPower;

            attackSpeed =
                satelliteData.attackSpeed;

            bullet =
                satelliteData.bullet;

            RecalculateAttackInterval();
        }

        private void ConnectSatelliteStatModule() {
            if (_satelliteStatModule != null)
                return;

            if (Eric.StageUpgrade.StageModuleOwner.Instance == null)
                return;

            _satelliteStatModule =
                Eric.StageUpgrade.StageModuleOwner.Instance
                    .GetModule<SatelliteStageStatModule>();
        }

        private int GetSharedAttackBonus() {
            if (_satelliteStatModule == null)
                return 0;

            return _satelliteStatModule.Attack -
                   _satelliteStatModule.BaseAttack;
        }

        private float GetSharedAttackSpeedBonus() {
            if (_satelliteStatModule == null)
                return 0f;

            return _satelliteStatModule.AttackSpeed -
                   _satelliteStatModule.BaseAttackSpeed;
        }

        public virtual void Deploy(Transform orbitCenter) {
            if (orbitCenter == null)
                return;

            ConnectSatelliteStatModule();
            RecalculateAttackInterval();

            MovementModule movementModule =
                GetModule<MovementModule>();

            if (movementModule == null)
                return;

            movementModule.SetCenter(orbitCenter);

            AttackModule attackModule =
                GetModule<AttackModule>();

            attackModule?.Activate();
        }

        public void Upgrade(int enforceLevel) {
            if (satelliteData == null)
                return;

            if (enforceLevel < 0 ||
                enforceLevel >=
                satelliteData.damageIncreaseAmount.Length ||
                enforceLevel >=
                satelliteData.attackSpeedIncreaseAmount.Length) {
                return;
            }

            float damageIncreaseRate =
                satelliteData
                    .damageIncreaseAmount[enforceLevel];

            float attackSpeedIncreaseRate =
                satelliteData
                    .attackSpeedIncreaseAmount[enforceLevel];

            attackPower = Mathf.RoundToInt(
                attackPower *
                (1f + damageIncreaseRate)
            );

            attackSpeed *=
                1f + attackSpeedIncreaseRate;

            RecalculateAttackInterval();
        }

        private void RecalculateAttackInterval() {
            float finalAttackSpeed =
                AttackSpeed;

            if (finalAttackSpeed <= 0f) {
                attackInterval =
                    float.PositiveInfinity;

                return;
            }

            attackInterval =
                1f / finalAttackSpeed;
        }
    }
}