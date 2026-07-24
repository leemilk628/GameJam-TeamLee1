using System.Collections;
using Key.Scripts.Feedback;
using Key.Scripts.Pooling;
using Key.Scripts.Projectile;
using Key.Scripts.Singletone;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Key.Scripts.Player {
    public class PlayerAttack : MonoBehaviour {
        public int AttackPower {
            get {
                int basePower = _playerStat != null
                    ? _playerStat.AttackPower
                    : baseAttackPower;

                return Mathf.Max(
                    0,
                    basePower + _bonusAttackPower
                );
            }
        }

        public float AttackSpeed {
            get {
                float baseSpeed = _playerStat != null
                    ? _playerStat.AttackSpeed
                    : baseAttackSpeed;

                return Mathf.Max(
                    0.01f,
                    (baseSpeed + _bonusAttackSpeed) *
                    _attackSpeedMultiplier
                );
            }
        }

        [Header("Pool")]
        [SerializeField] private BulletPoolManager bulletPoolManager;

        [Header("Fallback Stats")]
        [SerializeField] private int baseAttackPower = 10;
        [SerializeField, Min(0.01f)] private float baseAttackSpeed = 2f;
        [SerializeField, Min(0f)] private float fallbackKnockbackPower = 5f;

        [Header("Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Bullet")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField, Min(0f)] private float bulletSpread = 3f;

        [Header("Light")]
        [SerializeField] private Light2D light;
        [SerializeField, Min(0f)] private float shootLightDuration = 0.2f;

        [Header("Camera Shake")]
        [SerializeField] private CameraShake2D cameraShake;
        [SerializeField, Min(0f)] private float shootShakePower = 0.2f;

        [Header("Gun Recoil")]
        [SerializeField] private GunRecoil2D gunRecoil;

        private PlayerStat _playerStat;
        private Bullet _bullet;

        private int _bonusAttackPower;
        private float _bonusAttackSpeed;
        private float _attackSpeedMultiplier = 1f;

        private float _nextAttackTime;
        private Coroutine _shootLightCoroutine;
        private WaitForSeconds _shootLightWait;

        private void Awake() {
            if (mainCamera == null)
                mainCamera = Camera.main;

            _playerStat = GetComponent<PlayerStat>();

            if (_playerStat == null) {
                _playerStat =
                    FindFirstObjectByType<PlayerStat>();
            }

            if (_playerStat == null) {
                Debug.LogError(
                    $"{name}: PlayerStat을 찾을 수 없습니다.",
                    this
                );
            }

            if (bulletPoolManager == null) {
                bulletPoolManager =
                    FindFirstObjectByType<BulletPoolManager>();
            }

            if (light == null)
                light = GetComponentInChildren<Light2D>(true);

            if (light != null)
                light.enabled = false;

            if (cameraShake == null &&
                mainCamera != null) {
                cameraShake =
                    mainCamera.GetComponentInParent<CameraShake2D>();
            }

            if (gunRecoil == null) {
                gunRecoil =
                    GetComponentInChildren<GunRecoil2D>();
            }

            if (bulletPrefab != null) {
                _bullet =
                    bulletPrefab.GetComponent<Bullet>();
            }

            if (_bullet == null) {
                Debug.LogError(
                    $"{name}: Bullet 프리팹 또는 Bullet 컴포넌트가 없습니다.",
                    this
                );
            }

            if (firePoint == null) {
                Debug.LogError(
                    $"{name}: FirePoint가 설정되지 않았습니다.",
                    this
                );
            }

            _shootLightWait =
                new WaitForSeconds(shootLightDuration);
        }

        private void Update() {
            if (Mouse.current == null)
                return;

            if (Mouse.current.leftButton.isPressed)
                TryAttack();
        }

        private void TryAttack() {
            if (Time.time < _nextAttackTime)
                return;

            if (AttackSpeed <= 0f)
                return;

            if (bulletPoolManager == null ||
                _bullet == null ||
                _bullet.data == null ||
                firePoint == null) {
                return;
            }

            Vector2 shootDirection = (
                (Vector2)firePoint.position -
                (Vector2)transform.position
            ).normalized;

            if (shootDirection.sqrMagnitude <= 0.001f)
                return;

            float randomSpread = Random.Range(
                -bulletSpread,
                bulletSpread
            );

            Vector2 spreadDirection = Quaternion.Euler(
                0f,
                0f,
                randomSpread
            ) * shootDirection;

            Vector2 targetPosition =
                (Vector2)firePoint.position +
                spreadDirection * 100f;

            int bonusDamage =
                AttackPower - _bullet.data.Damage;

            float knockbackPower = _playerStat != null
                ? _playerStat.KnockbackPower
                : fallbackKnockbackPower;

            bulletPoolManager.SpawnBullet(
                _bullet,
                firePoint.position,
                targetPosition,
                _bullet.data,
                bonusDamage,
                knockbackPower
            );

            SoundManager.Instance?.PlaySFX(
                SoundType.PlayerShoot
            );

            gunRecoil?.PlayRecoil();

            if (_shootLightCoroutine != null)
                StopCoroutine(_shootLightCoroutine);

            _shootLightCoroutine =
                StartCoroutine(ShootLight());

            cameraShake?.AddShake(shootShakePower);

            _nextAttackTime =
                Time.time + 1f / AttackSpeed;
        }

        private IEnumerator ShootLight() {
            if (light == null)
                yield break;

            light.enabled = true;

            yield return _shootLightWait;

            light.enabled = false;
            _shootLightCoroutine = null;
        }

        public void AddAttackPower(int amount) {
            _bonusAttackPower += amount;
        }

        public void AddAttackSpeed(float amount) {
            _bonusAttackSpeed += amount;
        }

        public void MultiplyAttackSpeed(float multiplier) {
            _attackSpeedMultiplier = Mathf.Max(
                0.01f,
                _attackSpeedMultiplier * multiplier
            );
        }

        private void OnDisable() {
            if (_shootLightCoroutine != null) {
                StopCoroutine(_shootLightCoroutine);
                _shootLightCoroutine = null;
            }

            if (light != null)
                light.enabled = false;
        }
    }
}
