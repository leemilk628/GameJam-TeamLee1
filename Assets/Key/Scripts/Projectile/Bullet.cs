using Key.Scripts.BulletSc;
using Key.Scripts.Pooling;
using UnityEngine;

namespace Key.Scripts.Projectile {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Bullet : MonoBehaviour, IPoolable {
        [field:SerializeField] public BulletDataSO data { get; private set; }
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;

        private BulletPoolManager _poolManager;
        private BulletDataSO _data;
        private AreaDamageOnHit _areaDamageOnHit;

        private Vector2 _moveDirection;

        public int _damage { get; private set; }
        private float _knockbackPower;

        private float _returnTime;
        private bool _isActive;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _areaDamageOnHit = GetComponent<AreaDamageOnHit>();
        }

        private void Update() {
            if (!_isActive)
                return;

            if (Time.time >= _returnTime) {
                ReturnToPool();
            }
        }

        public void SetPoolManager(BulletPoolManager poolManager) {
            _poolManager = poolManager;
        }

        public void Shoot(Vector2 targetPosition, BulletDataSO data, int bonusDamage = 0, float bonusKnockback = 0f) {
            if (data == null) {
                Debug.LogError(
                    $"{name}: BulletDataSO가 전달되지 않았습니다.",
                    this
                );

                ReturnToPool();
                return;
            }

            _data = data;

            // SO 기본 수치 + 인공위성 강화 수치
            _damage = data.Damage + bonusDamage;

            _knockbackPower =
                data.KnockbackPower + bonusKnockback;

            _spriteRenderer.sprite = data.Sprite;

            Vector2 currentPosition = transform.position;

            _moveDirection =
                (targetPosition - currentPosition).normalized;

            if (_moveDirection.sqrMagnitude <= 0.001f) {
                ReturnToPool();
                return;
            }

            float angle =
                Mathf.Atan2(
                    _moveDirection.y,
                    _moveDirection.x
                ) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                angle + data.RotationOffset
            );

            _rigidbody.linearVelocity =
                _moveDirection * data.MoveSpeed;

            _returnTime =
                Time.time + data.LifeTime;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!_isActive)
                return;

            if (!other.CompareTag("Damageable"))
                return;

            if (_areaDamageOnHit != null) {
                Vector2 hitPosition =
                    other.ClosestPoint(transform.position);

                _areaDamageOnHit.Explode(
                    hitPosition,
                    _damage,
                    _knockbackPower
                );

                ReturnToPool();
                return;
            }

            IDamageable damageable =
                other.GetComponentInParent<IDamageable>();

            damageable?.GetDamage(_damage);

            IKnockbackable knockbackable =
                other.GetComponentInParent<IKnockbackable>();

            knockbackable?.Knockback(
                _moveDirection,
                _knockbackPower
            );

            ReturnToPool();
        }

        #region Pooling

        private void ReturnToPool() {
            if (!_isActive)
                return;

            _isActive = false;

            if (_poolManager != null) {
                _poolManager.ReturnBullet(this);
            }
            else {
                gameObject.SetActive(false);
            }
        }

        public void OnGetFromPool() {
            _isActive = true;

            _data = null;
            _damage = 0;
            _knockbackPower = 0f;
            _moveDirection = Vector2.zero;
            _returnTime = float.PositiveInfinity;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }

        public void OnReturnToPool() {
            _isActive = false;

            _data = null;
            _damage = 0;
            _knockbackPower = 0f;
            _moveDirection = Vector2.zero;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;

            transform.rotation = Quaternion.identity;

            // 풀 안에서 마지막 총알 이미지가 보일 일은 없지만
            // 상태를 확실하게 초기화
            _spriteRenderer.sprite = null;
        }

        #endregion
    }
}