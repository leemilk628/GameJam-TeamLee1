
using Key.Scripts;
using UnityEngine;

namespace LeeYoonWoo._01._Script.Enemy
{
    public class EnemyAttacker : MonoBehaviour
    {
        public void Attack(IDamageable target, int amount)
        {
            target.GetDamage(amount);
        }
    }
}