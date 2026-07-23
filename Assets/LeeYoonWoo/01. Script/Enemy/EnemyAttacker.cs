using LeeYoonWoo._01._Script.Interface;
using UnityEngine;

namespace LeeYoonWoo._01._Script.Enemy
{
    public class EnemyAttacker : MonoBehaviour
    {
        public void Attack(IDamageable target, int amount)
        {
            target.TakeDamage(amount);
        }
    }
}