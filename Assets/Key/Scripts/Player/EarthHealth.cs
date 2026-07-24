using Key.Scripts.Singletone;
using UnityEngine;

namespace Key.Scripts.Player {
    public class EarthHealth : MonoBehaviour, IDamageable {
        [field: SerializeField]
        public int Health { get; private set; }

        [field: SerializeField]
        public int Barrier { get; private set; }

        [SerializeField] private PlayerStat stat;

        private bool _isDead;

        private void Awake() {
            if (stat == null)
                stat = FindAnyObjectByType<PlayerStat>();
        }

        private void OnEnable() {
            _isDead = false;

            if (stat == null)
                return;

            stat.OnHealthChanged += HandleHealthChanged;
            stat.OnBarrierChanged += HandleBarrierChanged;

            Health = stat.CurrentHealth;
            Barrier = stat.CurrentBarrier;
        }

        private void OnDisable() {
            if (stat == null)
                return;

            stat.OnHealthChanged -= HandleHealthChanged;
            stat.OnBarrierChanged -= HandleBarrierChanged;
        }

        public void GetDamage(int damage) {
            if (_isDead ||
                damage <= 0 ||
                stat == null) {
                return;
            }

            stat.TakeDamage(damage);
        }

        public void AddBarrier(int amount) {
            if (_isDead ||
                amount <= 0 ||
                stat == null) {
                return;
            }

            stat.RestoreBarrier(amount);
        }

        public void Heal(int amount) {
            if (_isDead ||
                amount <= 0 ||
                stat == null) {
                return;
            }

            stat.Heal(amount);
        }

        private void HandleHealthChanged(
            int currentHealth,
            int maxHealth
        ) {
            Health = currentHealth;

            if (Health <= 0)
                Death();
        }

        private void HandleBarrierChanged(
            int currentBarrier,
            int maxBarrier
        ) {
            Barrier = currentBarrier;
        }

        public void Death() {
            if (_isDead)
                return;

            _isDead = true;

            if (GameManager.Instance == null) {
                return;
            }

            GameManager.Instance.GameOver();
        }
    }
}