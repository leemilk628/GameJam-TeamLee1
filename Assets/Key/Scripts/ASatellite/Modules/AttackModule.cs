using Key.Scripts.Pooling;
using Key.Scripts.Projectile;
using UnityEngine;

namespace Key.Scripts.ASatellite.Modules {
    public class AttackModule : MonoBehaviour, IModule {
        [Header("Attack")] 
        [SerializeField] private int attackPower = 10;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float attackRange = 5f;
        [SerializeField] private float knockbackPower = 3f;
        [SerializeField] private GameObject bulletPrefab;

        [Header("Target")] [SerializeField] private LayerMask targetLayer;

        [Header("Bullet")] 
        [SerializeField] private Transform firePoint;
        [SerializeField] private BulletPoolManager bulletPoolManager;

        private AbstractASatellite _owner;
        private Collider2D _ownerCollider;
        private Transform _nearest;

        private float _attackTimer;
        private bool _isActive;

        public void Initialize(ModuleOwner owner) {
            _owner = owner as AbstractASatellite;

            if (_owner == null) return;
            
            _ownerCollider =
                _owner.GetComponentInChildren<Collider2D>();

            if (firePoint == null) {
                firePoint = _owner.transform;
            }

            if (bulletPoolManager == null) {
                bulletPoolManager =
                    FindFirstObjectByType<BulletPoolManager>();
            }

            _owner.OnTick += Tick;
        }

        private void Tick(float deltaTime) {
            if (!_isActive)
                return;

            if (attackSpeed <= 0f)
                return;

            _attackTimer -= deltaTime;

            if (_attackTimer > 0f)
                return;

            Collider2D target = FindClosestTarget();

            if (target == null)
                return;

            Fire(target);

            _attackTimer = 1f / attackSpeed;
        }

        private void Fire(Collider2D target) {
            if (bulletPoolManager == null || firePoint == null)
                return;

            Vector2 targetPosition = target.bounds.center;

            bulletPoolManager.SpawnBullet(
                firePoint.position,
                _nearest.position,
                bulletPrefab.GetComponent<Bullet>().data
            );
        }

        private Collider2D FindClosestTarget() {
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
