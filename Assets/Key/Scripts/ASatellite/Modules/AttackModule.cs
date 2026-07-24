using Key.Scripts.BulletSc;
using Key.Scripts.Pooling;
using Key.Scripts.Projectile;
using UnityEngine;

namespace Key.Scripts.ASatellite.Modules {
    public class AttackModule : MonoBehaviour, IModule {
        [Header("Attack")]
        [SerializeField, Min(0.1f)] private float attackRange = 5f;
        [SerializeField, Min(0f)] private float knockbackPower = 3f;

        [Header("Target")]
        [SerializeField] private LayerMask targetLayer;

        [Header("Bullet")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private BulletPoolManager bulletPoolManager;

        private AbstractASatellite _owner;
        private Collider2D _ownerCollider;

        private float _attackTimer;
        private bool _isActive;

        public void Initialize(ModuleOwner owner) {
            _owner = owner as AbstractASatellite;

            if (_owner == null) {
                Debug.LogError(
                    $"{name}: ModuleOwner가 AbstractASatellite이 아닙니다.",
                    this
                );

                return;
            }

            _ownerCollider =
                _owner.GetComponentInChildren<Collider2D>();

            if (firePoint == null) {
                firePoint = _owner.transform;
            }

            if (bulletPoolManager == null) {
                bulletPoolManager =
                    FindFirstObjectByType<BulletPoolManager>();
            }

            if (bulletPoolManager == null) {
                Debug.LogError(
                    $"{name}: BulletPoolManager를 찾을 수 없습니다.",
                    this
                );
            }

            _owner.OnTick += Tick;
        }

        private void Tick(float deltaTime) {
            if (!_isActive || _owner == null)
                return;

            if (_owner.AttackSpeed <= 0f)
                return;

            _attackTimer -= deltaTime;

            if (_attackTimer > 0f)
                return;

            Collider2D target = FindClosestTarget();

            if (target == null)
                return;

            Fire(target);

            _attackTimer = _owner.AttackInterval;
        }

        private void Fire(Collider2D target) {
            if (target == null ||
                bulletPoolManager == null ||
                firePoint == null ||
                _owner == null) {
                return;
            }

            GameObject bulletPrefab =
                _owner.BulletPrefab;

            if (bulletPrefab == null) {
                Debug.LogError(
                    $"{_owner.name}: ASatelliteSO에 총알 프리팹이 설정되지 않았습니다.",
                    _owner
                );

                return;
            }

            Bullet bullet =
                bulletPrefab.GetComponent<Bullet>();

            if (bullet == null) {
                Debug.LogError(
                    $"{bulletPrefab.name}: Bullet 컴포넌트가 없습니다.",
                    bulletPrefab
                );

                return;
            }

            BulletDataSO bulletData =
                bullet.data;

            if (bulletData == null) {
                Debug.LogError(
                    $"{bulletPrefab.name}: BulletDataSO가 설정되지 않았습니다.",
                    bulletPrefab
                );

                return;
            }

            bulletPoolManager.SpawnBullet(
                firePoint.position,
                target.bounds.center,
                bulletData,
                _owner.AttackPower,
                knockbackPower
            );
        }

        private Collider2D FindClosestTarget() {
            if (_owner == null)
                return null;

            Collider2D[] targets =
                Physics2D.OverlapCircleAll(
                    _owner.transform.position,
                    attackRange,
                    targetLayer
                );

            Collider2D closestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (Collider2D target in targets) {
                if (target == null)
                    continue;

                if (target == _ownerCollider)
                    continue;

                if (target.transform.IsChildOf(_owner.transform))
                    continue;

                float distance =
                    ((Vector2)target.bounds.center -
                     (Vector2)_owner.transform.position)
                    .sqrMagnitude;

                if (distance >= closestDistance)
                    continue;

                closestDistance = distance;
                closestTarget = target;
            }

            return closestTarget;
        }

        public void Activate() {
            _isActive = true;
            _attackTimer = 0f;
        }

        public void Deactivate() {
            _isActive = false;
            _attackTimer = 0f;
        }

        private void OnDestroy() {
            if (_owner != null) {
                _owner.OnTick -= Tick;
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(
                transform.position,
                attackRange
            );
        }
    }
}