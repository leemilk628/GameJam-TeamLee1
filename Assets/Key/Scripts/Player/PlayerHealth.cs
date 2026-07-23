using UnityEngine;

namespace Key.Scripts.Player {
    public class PlayerHealth : MonoBehaviour, IDamageable {
        public int Health { get; private set; }

        [SerializeField] private int maxHealth;

        private int _baseBarrier;
        private int _barrier;

        private void OnEnable() {
            GetBarrier(_baseBarrier);
            Health = maxHealth;
        }

        private void GetBarrier(int amount) {
            _barrier = amount;
        }

        public void GetDamage(int damage) {
            Health -= damage;
        }
    }
}