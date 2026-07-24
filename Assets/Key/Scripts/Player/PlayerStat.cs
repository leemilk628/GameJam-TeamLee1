using System;
using System.Collections;
using Eric.Player;
using Eric.StageUpgrade;
using UnityEngine;
using UnityEngine.Serialization;

namespace Key.Scripts.Player {
    public class PlayerStat : MonoBehaviour {
        [Header("Fallback Stat")]
        [FormerlySerializedAs("<AttackPower>k__BackingField")]
        [SerializeField] private int fallbackAttackPower = 10;
        [FormerlySerializedAs("<MaxHealth>k__BackingField")]
        [SerializeField] private int fallbackMaxHealth = 10;
        [FormerlySerializedAs("<Barrier>k__BackingField")]
        [SerializeField] private int fallbackMaxBarrier = 10;
        [FormerlySerializedAs("<AttackSpeed>k__BackingField")]
        [SerializeField] private float fallbackAttackSpeed = 2f;
        [SerializeField] private int fallbackBarrierRecoverySpeed = 1;

        [field: SerializeField]
        public float KnockbackPower { get; private set; } = 5f;

        private PlayerStageStatModule _playerStatModule;
        private StageUpgradeModule _stageUpgradeModule;

        public int AttackPower =>
            _playerStatModule != null
                ? _playerStatModule.Attack
                : _stageUpgradeModule != null
                    ? _stageUpgradeModule.GetPlayerAttack(
                        fallbackAttackPower
                    )
                    : fallbackAttackPower;

        public int MaxHealth =>
            _playerStatModule != null
                ? _playerStatModule.MaxHealth
                : _stageUpgradeModule != null
                    ? _stageUpgradeModule.GetPlayerMaxHealth(
                        fallbackMaxHealth
                    )
                    : fallbackMaxHealth;

        public int CurrentHealth =>
            _playerStatModule != null
                ? _playerStatModule.CurrentHealth
                : MaxHealth;

        public int Barrier =>
            _playerStatModule != null
                ? _playerStatModule.MaxBarrier
                : _stageUpgradeModule != null
                    ? _stageUpgradeModule.GetPlayerBarrier(
                        fallbackMaxBarrier
                    )
                    : fallbackMaxBarrier;

        public int CurrentBarrier =>
            _playerStatModule != null
                ? _playerStatModule.CurrentBarrier
                : Barrier;

        public float AttackSpeed =>
            _playerStatModule != null
                ? _playerStatModule.AttackSpeed
                : _stageUpgradeModule != null
                    ? _stageUpgradeModule.GetPlayerAttackSpeed(
                        Mathf.RoundToInt(fallbackAttackSpeed)
                    )
                    : fallbackAttackSpeed;

        public int BarrierRecoverySpeed =>
            _playerStatModule != null
                ? _playerStatModule.BarrierRecoverySpeed
                : _stageUpgradeModule != null
                    ? _stageUpgradeModule
                        .GetPlayerBarrierRecoverySpeed(
                            fallbackBarrierRecoverySpeed
                        )
                    : fallbackBarrierRecoverySpeed;

        public bool IsConnected =>
            _playerStatModule != null ||
            _stageUpgradeModule != null;

        public event Action OnStatsChanged;
        public event Action<int, int> OnHealthChanged;
        public event Action<int, int> OnBarrierChanged;

        private IEnumerator Start() {
            while (StageModuleOwner.Instance == null)
                yield return null;

            StageModuleOwner owner =
                StageModuleOwner.Instance;

            _playerStatModule =
                owner.GetModule<PlayerStageStatModule>();

            _stageUpgradeModule =
                owner.GetModule<StageUpgradeModule>();

            if (_playerStatModule != null) {
                _playerStatModule.OnStatsChanged += HandleStatsChanged;
                _playerStatModule.OnHealthChanged += HandleHealthChanged;
                _playerStatModule.OnBarrierChanged += HandleBarrierChanged;
            } else if (_stageUpgradeModule != null) {
                _stageUpgradeModule.OnStageUpgradeDataChanged +=
                    HandleStatsChanged;
            } else {
                Debug.LogWarning(
                    $"{name}: 플레이어 스탯 모듈과 스테이지 " +
                    "업그레이드 모듈을 찾을 수 없어 기본값을 사용합니다.",
                    this
                );
            }

            HandleStatsChanged();

            HandleHealthChanged(
                CurrentHealth,
                MaxHealth
            );

            HandleBarrierChanged(
                CurrentBarrier,
                Barrier
            );
        }

        public void TakeDamage(int damage) {
            if (_playerStatModule == null)
                return;

            _playerStatModule.TakeDamage(damage);
        }

        public void Heal(int amount) {
            if (_playerStatModule == null)
                return;

            _playerStatModule.Heal(amount);
        }

        public void RestoreBarrier(int amount) {
            if (_playerStatModule == null)
                return;

            _playerStatModule.RestoreBarrier(amount);
        }

        public void ResetCurrentStats() {
            if (_playerStatModule == null)
                return;

            _playerStatModule.ResetCurrentStats();
        }

        public void AddKnockbackPower(float amount) {
            KnockbackPower = Mathf.Max(
                0f,
                KnockbackPower + amount
            );
        }

        private void HandleStatsChanged() {
            OnStatsChanged?.Invoke();
        }

        private void HandleHealthChanged(
            int currentHealth,
            int maxHealth
        ) {
            OnHealthChanged?.Invoke(
                currentHealth,
                maxHealth
            );
        }

        private void HandleBarrierChanged(
            int currentBarrier,
            int maxBarrier
        ) {
            OnBarrierChanged?.Invoke(
                currentBarrier,
                maxBarrier
            );
        }

        private void OnDestroy() {
            if (_playerStatModule != null) {
                _playerStatModule.OnStatsChanged -= HandleStatsChanged;
                _playerStatModule.OnHealthChanged -= HandleHealthChanged;
                _playerStatModule.OnBarrierChanged -= HandleBarrierChanged;
            }

            if (_stageUpgradeModule != null) {
                _stageUpgradeModule.OnStageUpgradeDataChanged -=
                    HandleStatsChanged;
            }
        }
    }
}
