using LeeYoonWoo._01._Script.Enemy;
using LeeYoonWoo._01._Script.Interface;
using UnityEngine;

namespace LeeYoonWoo._01._Script.Abstract
{
    public class AbstractEnemy : MonoBehaviour, IDamageable
    {
        protected EnemyMovement _mvm;
        protected EnemyAttacker _atk;

        protected virtual void Awake()
        {
            _mvm = GetComponent<EnemyMovement>();
            _atk = GetComponent<EnemyAttacker>();
        }
    }
}