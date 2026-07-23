using Key.Scripts.ASatellite.Modules;
using UnityEngine;

namespace Key.Scripts {
    public class Earth : ModuleOwner {
        [SerializeField] private int health;

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("damaging")) return;
            
            
        }
    }
}