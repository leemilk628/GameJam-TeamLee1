using Key.Scripts.Projectile;
using UnityEngine;

namespace Key.Scripts.Player {
    public class Earth : MonoBehaviour, IDamageable {
        [field:SerializeField] public int Health { get; private set; }

        [SerializeField] private int maxHealth;

        private int _baseBarrier;
        private int _barrier;
        private PlayerStat _stat;

        private void Awake() {
            _stat = GetComponent<PlayerStat>();
        }

        private void OnEnable() {
            GetBarrier(_baseBarrier);
            Health = maxHealth;
        }

        private void GetBarrier(int amount) {
            _barrier = amount;
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Damaging")) return;

            int dam = other.gameObject.GetComponent<Bullet>()._damage;
            GetDamage(dam);
        }

        public void GetDamage(int damage) {
            Health -= damage;
        }

        public void Death() {
            Time.timeScale = 0f;
            //Gameover 여기에 구현하깅
        }
    }
}