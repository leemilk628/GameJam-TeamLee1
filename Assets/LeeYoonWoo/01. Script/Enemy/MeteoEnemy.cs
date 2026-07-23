using LeeYoonWoo._01._Script.Abstract;
using LeeYoonWoo._01._Script.Interface;
using Unity.VisualScripting;
using UnityEngine;

namespace LeeYoonWoo._01._Script.Enemy
{
    public class MeteoEnemy : AbstractEnemy
    {
        [SerializeField] private GameObject mvdir;
        
        void Start()
        {
            mvdir = GameObject.FindGameObjectWithTag("Player");
            _mvm.Move(mvdir.transform.position);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamageable player))
            {
                _atk.Attack(player, 10);
            }
        }
    }
}