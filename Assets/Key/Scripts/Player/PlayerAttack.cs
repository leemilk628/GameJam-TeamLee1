using System.Collections;
using Key.Scripts.Pooling;
using Key.Scripts.Projectile;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using Key.Scripts.Feedback;

namespace Key.Scripts.Player {
    public class PlayerAttack : MonoBehaviour {
        public int AttackPower { get; private set; }
        [field: SerializeField] public float AttackSpeed { get; private set; }

        [Header("Pool")] [SerializeField] private BulletPoolManager bulletPoolManager;

        [Header("Stats")] [SerializeField] private int baseAttackPower = 10;

        // 초당 발사 횟수
        [SerializeField] private float baseAttackSpeed = 2f;

        [Header("Camera")] [SerializeField] private Camera mainCamera;

        [Header("Bullet")] [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;

        [Header("Light")] [SerializeField] private Light2D light;
        [SerializeField] private WaitForSeconds shutDownTime = new WaitForSeconds(0.2f);

        [Header("Camera Shake")] [SerializeField]
        private CameraShake2D cameraShake;

        [SerializeField] private float shootShakePower = 0.2f;
        
        [Header("Gun Recoil")]
        [SerializeField] private GunRecoil2D gunRecoil;


        private float _nextAttackTime;
        private PlayerStat _stat;
        private float _knockbackPower;
        private PlayerStat _playerStat;
        private float _bulletSpread = 3f;
        private Coroutine _shootLightCoroutine;
        private Bullet bullet;

        private void Awake() {
            if (mainCamera == null) {
                mainCamera = Camera.main;
            }

            if (_playerStat == null) {
                _playerStat = GetComponent<PlayerStat>();
            }

            if (bulletPoolManager == null) {
                bulletPoolManager =
                    FindFirstObjectByType<BulletPoolManager>();
            }

            AttackPower = baseAttackPower;
            AttackSpeed = baseAttackSpeed;

            if (light == null)
                light = GetComponentInChildren<Light2D>(true);

            if (light != null)
                light.enabled = false;

            if (cameraShake == null && mainCamera != null)
                cameraShake = mainCamera.GetComponentInParent<CameraShake2D>();
            
            if (gunRecoil == null)
                gunRecoil = GetComponentInChildren<GunRecoil2D>();

            bullet = bulletPrefab.GetComponent<Bullet>();
        }

        private void Update() {
            if (Mouse.current == null)
                return;

            if (Mouse.current.leftButton.isPressed) {
                TryAttack();
            }
        }

        private void TryAttack() {
            if (Time.time < _nextAttackTime)
                return;

            if (AttackSpeed <= 0f) 
                return;
            
            if (bulletPoolManager == null || bulletPrefab == null) 
                return;

            Vector2 shootDirection = (
                (Vector2)firePoint.position -
                (Vector2)transform.position
            ).normalized;

            if (shootDirection == Vector2.zero)
                return;

            float randomSpread = Random.Range(
                -_bulletSpread,
                _bulletSpread
            );

            Vector2 spreadDirection = Quaternion.Euler(
                0f,
                0f,
                randomSpread
            ) * shootDirection;

            Vector2 targetPosition =
                (Vector2)firePoint.position +
                spreadDirection * 100f;

            bulletPoolManager.SpawnBullet(
                bullet,
                firePoint.position,
                targetPosition,
                bullet.data,
                AttackPower,
                _knockbackPower
            );
            
            gunRecoil?.PlayRecoil();

            if (_shootLightCoroutine != null)
                StopCoroutine(_shootLightCoroutine);

            _shootLightCoroutine = StartCoroutine(ShootLight());

            cameraShake?.AddShake(shootShakePower);

            _nextAttackTime = Time.time + (1f / AttackSpeed);
        }

        private IEnumerator ShootLight() {
            if (light == null)
                yield break;

            light.enabled = true;

            yield return shutDownTime;

            light.enabled = false;
            _shootLightCoroutine = null;
        }

        public void AddAttackPower(int amount) {
            AttackPower += amount;
        }

        public void AddAttackSpeed(float amount) {
            AttackSpeed = Mathf.Max(
                0.01f,
                AttackSpeed + amount
            );
        }

        public void MultiplyAttackSpeed(float multiplier) {
            AttackSpeed = Mathf.Max(
                0.01f,
                AttackSpeed * multiplier
            );
        }
    }
}