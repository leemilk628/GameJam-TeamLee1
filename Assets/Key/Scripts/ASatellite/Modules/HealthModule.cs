using Key.Scripts.Pooling;
using UnityEngine;

namespace Key.Scripts.ASatellite.Modules {
    public class HealthModule : MonoBehaviour, IModule {
        public int Health { get; private set; }

        private Transform _owner;
        
        public void Initialize(ModuleOwner owner) {
            _owner = owner.transform;
        }

        public void GetDamage(int damage) {
            Health -= damage;

            if (Health <= 0)
                Death();
        }

        private void Death() {
            IPoolable isPool = _owner.gameObject.GetComponent<IPoolable>();
            
            if(isPool == null) 
                Destroy(gameObject);
            
            _owner.gameObject.GetComponent<IPoolable>().OnReturnToPool(); 
        }
    }
}