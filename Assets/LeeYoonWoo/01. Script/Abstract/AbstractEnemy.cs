using Key.Scripts;
using LeeYoonWoo._01._Script.Enemy;
using UnityEngine;

namespace LeeYoonWoo._01._Script.Abstract
{
    public class AbstractEnemy : MonoBehaviour, IDamageable
    {
        protected EnemyMovement _mvm;
        protected EnemyAttacker _atk;
        protected float Health;

        protected virtual void Awake()
        {
            _mvm = GetComponent<EnemyMovement>();
            _atk = GetComponent<EnemyAttacker>();
        }

        protected virtual void Update()
        {
            if (Health <= 0)
            {
                Death();
            }
        }

        public void GetDamage(int damage)
        {
            Health -= damage;
        }

        public virtual void Death()
        {
        }
    }
}