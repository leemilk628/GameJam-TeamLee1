using System;
using System.Collections;
using Eric.Player;
using Eric.StageUpgrade;
using UnityEngine;

namespace Key.Scripts.Player {
    public class PlayerStat : MonoBehaviour {
        [Header("Fallback Stat")]
        [SerializeField] private int fallbackAttackPower = 10;
        [SerializeField] private int fallbackMaxHealth = 10;
        [SerializeField] private int fallbackMaxBarrier = 10;
        [SerializeField] private float fallbackAttackSpeed = 2f;

        [field: SerializeField]
        public float KnockbackPower { get; private set; } = 5f;

        private PlayerStageStatModule _playerStatModule;

        public int AttackPower =>
            _playerStatModule != null
                ? _playerStatModule.Attack
                : fallbackAttackPower;

        public int MaxHealth =>
            _playerStatModule != null
                ? _playerStatModule.MaxHealth
                : fallbackMaxHealth;

        public int CurrentHealth =>
            _playerStatModule != null
                ? _playerStatModule.CurrentHealth
                : fallbackMaxHealth;

        public int Barrier =>
            _playerStatModule != null
                ? _playerStatModule.MaxBarrier
                : fallbackMaxBarrier;

        public int CurrentBarrier =>
            _playerStatModule != null
                ? _playerStatModule.CurrentBarrier
                : fallbackMaxBarrier;

        public float AttackSpeed =>
            _playerStatModule != null
                ? _playerStatModule.AttackSpeed
                : fallbackAttackSpeed;

        public int BarrierRecoverySpeed =>
            _playerStatModule != null
                ? _playerStatModule.BarrierRecoverySpeed
                : 0;

        public bool IsConnected =>
            _playerStatModule != null;

        public event Action OnStatsChanged;
        public event Action<int, int> OnHealthChanged;
        public event Action<int, int> OnBarrierChanged;

        private IEnumerator Start() {
            while (StageModuleOwner.Instance == null)
                yield return null;

            while (_playerStatModule == null) {
                _playerStatModule =
                    StageModuleOwner.Instance
                        .GetModule<PlayerStageStatModule>();

                if (_playerStatModule == null)
                    yield return null;
            }

            _playerStatModule.OnStatsChanged += HandleStatsChanged;
            _playerStatModule.OnHealthChanged += HandleHealthChanged;
            _playerStatModule.OnBarrierChanged += HandleBarrierChanged;

            HandleStatsChanged();

            HandleHealthChanged(
                _playerStatModule.CurrentHealth,
                _playerStatModule.MaxHealth
            );

            HandleBarrierChanged(
                _playerStatModule.CurrentBarrier,
                _playerStatModule.MaxBarrier
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
            if (_playerStatModule == null)
                return;

            _playerStatModule.OnStatsChanged -= HandleStatsChanged;
            _playerStatModule.OnHealthChanged -= HandleHealthChanged;
            _playerStatModule.OnBarrierChanged -= HandleBarrierChanged;
        }
    }
}