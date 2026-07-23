using Key.Scripts.Pooling;
using Key.Scripts.Projectile;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Key.Scripts.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        public int AttackPower { get; private set; }
        [field:SerializeField]public float AttackSpeed { get; private set; }

        [Header("Pool")]
        [SerializeField] private BulletPoolManager bulletPoolManager;
        
        [Header("Stats")]
        [SerializeField] private int baseAttackPower = 10;

        // 초당 발사 횟수
        [SerializeField] private float baseAttackSpeed = 2f;

        [Header("Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Bullet")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;

        private float _nextAttackTime;
        private PlayerStat _stat;
        private float _knockbackPower;
        private PlayerStat _playerStat;
        
        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (firePoint == null)
            {
                firePoint = transform;
            }

            if (_playerStat == null)
            {
                _playerStat = GetComponent<PlayerStat>();
            }

            if (bulletPoolManager == null)
            {
                bulletPoolManager =
                    FindFirstObjectByType<BulletPoolManager>();
            }
        }

        private void Update()
        {
            if (Mouse.current == null)
                return;

            if (Mouse.current.leftButton.isPressed)
            {
                TryAttack();
            }
        }

        private void TryAttack()
        {
            if (Time.time < _nextAttackTime)
                return;

            if (AttackSpeed <= 0f)
                return;

            if (mainCamera == null || bulletPrefab == null) return;

            Vector2 mouseScreenPosition =
                Mouse.current.position.ReadValue();

            float distanceFromCamera = Mathf.Abs(
                firePoint.position.z - mainCamera.transform.position.z
            );

            Vector3 mouseWorldPosition3D =
                mainCamera.ScreenToWorldPoint(
                    new Vector3(
                        mouseScreenPosition.x,
                        mouseScreenPosition.y,
                        distanceFromCamera
                    )
                );

            Vector2 mouseWorldPosition =
                new Vector2(
                    mouseWorldPosition3D.x,
                    mouseWorldPosition3D.y
                );

            bulletPoolManager.SpawnBullet(
                firePoint.position,
                mouseWorldPosition,
                bulletPrefab.GetComponent<Bullet>().data
            );
            
            _nextAttackTime = Time.time + (1f / AttackSpeed);
        }

        public void AddAttackPower(int amount)
        {
            AttackPower += amount;
        }

        public void AddAttackSpeed(float amount)
        {
            AttackSpeed = Mathf.Max(
                0.01f,
                AttackSpeed + amount
            );
        }

        public void MultiplyAttackSpeed(float multiplier)
        {
            AttackSpeed = Mathf.Max(
                0.01f,
                AttackSpeed * multiplier
            );
        }
    }
}