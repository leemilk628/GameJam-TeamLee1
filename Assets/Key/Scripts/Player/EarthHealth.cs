using System;
using Key.Scripts.Singletone;
using UnityEngine;

namespace Key.Scripts.Player {
    public class EarthHealth : MonoBehaviour, IDamageable {
        [field: SerializeField]
        public int Health { get; private set; }

        [field: SerializeField]
        public int Barrier { get; private set; }

        public int MaxHealth =>
            _stat != null
                ? _stat.MaxHealth
                : 0;

        public int MaxBarrier =>
            _stat != null
                ? _stat.Barrier
                : 0;

        public event Action<int, int> OnHealthChanged;
        public event Action<int, int> OnBarrierChanged;

        [SerializeField] private PlayerStat stat;

        private PlayerStat _stat;
        private float _barrierRecoveryTimer;
        private bool _isDead;

        private void Awake() {
            _stat = stat;

            if (_stat == null)
                _stat = GetComponent<PlayerStat>();

            if (_stat == null)
                _stat = FindFirstObjectByType<PlayerStat>();

            if (_stat == null) {
                Debug.LogError(
                    $"{name}: PlayerStat을 찾을 수 없습니다.",
                    this
                );
            }
        }

        private void Start() {
            InitializeHealth();

            if (_stat != null)
                _stat.OnStatsChanged += HandleStatsChanged;
        }

        private void Update() {
            RecoverBarrier();
        }

        private void InitializeHealth() {
            if (_stat == null)
                return;

            Health = _stat.MaxHealth;
            Barrier = _stat.Barrier;

            _barrierRecoveryTimer = 0f;
            _isDead = false;

            NotifyHealthChanged();
            NotifyBarrierChanged();
        }

        public void GetDamage(int damage) {
            if (_isDead ||
                damage <= 0 ||
                _stat == null) {
                return;
            }

            int remainingDamage = damage;

            if (Barrier > 0) {
                int barrierDamage = Mathf.Min(
                    Barrier,
                    remainingDamage
                );

                Barrier -= barrierDamage;
                remainingDamage -= barrierDamage;

                NotifyBarrierChanged();
            }

            if (remainingDamage > 0) {
                Health = Mathf.Max(
                    0,
                    Health - remainingDamage
                );

                NotifyHealthChanged();
            }

            _barrierRecoveryTimer = 0f;

            SoundManager.Instance?.PlaySFX(
                SoundType.PlayerHit
            );

            if (Health <= 0)
                Death();
        }

        public void Heal(int amount) {
            if (_isDead ||
                amount <= 0 ||
                _stat == null) {
                return;
            }

            int previousHealth = Health;

            Health = Mathf.Min(
                Health + amount,
                _stat.MaxHealth
            );

            if (Health != previousHealth)
                NotifyHealthChanged();
        }

        public void AddBarrier(int amount) {
            if (_isDead ||
                amount <= 0 ||
                _stat == null) {
                return;
            }

            int previousBarrier = Barrier;

            Barrier = Mathf.Min(
                Barrier + amount,
                _stat.Barrier
            );

            if (Barrier != previousBarrier)
                NotifyBarrierChanged();
        }

        private void RecoverBarrier() {
            if (_isDead ||
                _stat == null ||
                Health <= 0 ||
                Barrier >= _stat.Barrier ||
                _stat.BarrierRecoverySpeed <= 0) {
                _barrierRecoveryTimer = 0f;
                return;
            }

            _barrierRecoveryTimer += Time.deltaTime;

            if (_barrierRecoveryTimer < 1f)
                return;

            int recoveryCount =
                Mathf.FloorToInt(_barrierRecoveryTimer);

            _barrierRecoveryTimer -= recoveryCount;

            AddBarrier(
                _stat.BarrierRecoverySpeed *
                recoveryCount
            );
        }

        private void HandleStatsChanged() {
            if (_stat == null)
                return;

            Health = Mathf.Clamp(
                Health,
                0,
                _stat.MaxHealth
            );

            Barrier = Mathf.Clamp(
                Barrier,
                0,
                _stat.Barrier
            );

            NotifyHealthChanged();
            NotifyBarrierChanged();
        }

        private void NotifyHealthChanged() {
            OnHealthChanged?.Invoke(
                Health,
                MaxHealth
            );
        }

        private void NotifyBarrierChanged() {
            OnBarrierChanged?.Invoke(
                Barrier,
                MaxBarrier
            );
        }

        public void Death() {
            if (_isDead)
                return;

            _isDead = true;

            GameManager.Instance?.GameOver();
        }

        private void OnDestroy() {
            if (_stat != null)
                _stat.OnStatsChanged -= HandleStatsChanged;
        }
    }
}