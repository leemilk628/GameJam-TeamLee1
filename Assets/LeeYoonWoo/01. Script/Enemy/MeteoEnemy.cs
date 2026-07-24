using System.Collections;
using DG.Tweening;
using Eric.DropItems;
using Key.Scripts;
using LeeYoonWoo._01._Script.Abstract;
using UnityEngine;

namespace LeeYoonWoo._01._Script.Enemy
{
    public class MeteoEnemy : AbstractEnemy
    {
        [SerializeField] private GameObject mvdir;
        [SerializeField] private int damage;
        
        
        void Start()
        {
            Health = 100;
            mvdir = GameObject.FindGameObjectWithTag("Player");
            _mvm.Move(mvdir.transform.position);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Damageable"))
                return;
            
            if (other.TryGetComponent(out IDamageable player))
            {
                Debug.Log(other.name);
                player.GetDamage(damage);
            }
            
        }

        public override void Death()
        {
            StartCoroutine(DeathRoutine());
        }

        IEnumerator DeathRoutine()
        {
            
            CreateObjects.Instance.CreateEvent(5, 13, transform.position);
            
            gameObject.layer = LayerMask.NameToLayer("DieObj");
            var sr = GetComponent<SpriteRenderer>();
            sr.DOColor(new Color(1, 1, 1, 0), 0.4f);
            
            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            
            yield return new WaitForSeconds(0.3f);
            var ps = GetComponentsInChildren<ParticleSystem>();
            foreach (var p in ps)
            {
                p.Stop();
            }
            yield  return new WaitForSeconds(1.2f);
            Destroy(gameObject);
        }
    }
}