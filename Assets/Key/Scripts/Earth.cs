using Key.Scripts.Projectile;
using UnityEngine;

namespace Key.Scripts {
    public class Earth : MonoBehaviour, IDamageable {
        [SerializeField] private int health;

       

        public void GetDamage(int damage) {
            health -= damage;

            if (health <= 0) {
                Death();
            }
        }

        public void Death() {
            
        }
    }
}