using Key.Scripts.Projectile;
using Key.Scripts.Singletone;
using UnityEngine;

namespace Key.Scripts.Player {
    public class EarthHealth : MonoBehaviour, IDamageable {
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public int Barrier { get; private set; }

        [Header("UI")]
        [SerializeField] private GameObject deathUI;

        private PlayerStat _stat;
        private bool _isDead;

        private void Awake() {
            _stat = GetComponent<PlayerStat>();

            if (_stat == null)
                return;

            if (deathUI != null)
                deathUI.SetActive(false);
        }

        private void OnEnable() {
            InitializeHealth();
        }

        private void InitializeHealth() {
            if (_stat == null)
                return;

            Health = _stat.MaxHealth;
            Barrier = _stat.Barrier;

            _isDead = false;

            if (deathUI != null)
                deathUI.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (_isDead)
                return;

            if (!other.CompareTag("Damaging"))
                return;

            Bullet bullet = other.GetComponentInParent<Bullet>();

            if (bullet == null)
                return;

            GetDamage(bullet._damage);
        }

        public void GetDamage(int damage) {
            if (_isDead || damage <= 0)
                return;

            int remainingDamage = damage;

            if (Barrier > 0) {
                int absorbedDamage = Mathf.Min(Barrier, remainingDamage);

                Barrier -= absorbedDamage;
                remainingDamage -= absorbedDamage;
            }

            if (remainingDamage > 0)
                Health = Mathf.Max(0, Health - remainingDamage);

            if (Health <= 0)
                Death();
        }

        public void AddBarrier(int amount) {
            if (amount <= 0)
                return;

            Barrier += amount;
        }

        public void Heal(int amount) {
            if (_isDead || amount <= 0 || _stat == null)
                return;

            Health = Mathf.Min(
                Health + amount,
                _stat.MaxHealth
            );
        }

        public void Death() {
            if (_isDead)
                return;

            _isDead = true;

            if (deathUI != null)
                deathUI.SetActive(true);
            else return;

            GameManager.Instance.GameOver();
        }
    }
}