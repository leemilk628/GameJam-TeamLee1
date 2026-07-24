using LeeYoonWoo._01._Script.Interface;
using UnityEngine;

namespace LeeYoonWoo._01._Script.Enemy
{
    public class EnemyMovement : MonoBehaviour, ISpinable
    {
        public Rigidbody2D rb { get; private set; }
        public Vector2 moveDir;
        [SerializeField] private float speed;
        [field: SerializeField] public float rotSpeed { get; set; }
        private float rot;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            rb.linearVelocity = moveDir * speed;
            transform.Rotate(0f, 0f, rotSpeed * Time.deltaTime);
        }

        public void Move(Vector2 pos)
        {
            moveDir = (pos-(Vector2)transform.position).normalized;
        }

    }
}