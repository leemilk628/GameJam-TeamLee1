using System.Collections.Generic;
using Key.Scripts.BulletSc;
using Key.Scripts.Projectile;
using UnityEngine;

namespace Key.Scripts.Pooling {
    public class BulletPoolManager : MonoBehaviour {
        [Header("Pool")]
        [SerializeField] private Bullet bulletPrefab;

        [Min(1)]
        [SerializeField] private int initialPoolSize = 30;

        [SerializeField] private bool canExpand = true;

        private readonly Queue<Bullet> _bulletPool = new();

        private void Awake() {
            CreateInitialPool();
        }

        private void CreateInitialPool() {
            if (bulletPrefab == null) {
                Debug.LogError(
                    "BulletPoolManager에 Bullet Prefab이 설정되지 않았습니다.",
                    this
                );

                return;
            }

            for (int i = 0; i < initialPoolSize; i++) {
                Bullet bullet = CreateBullet();
                _bulletPool.Enqueue(bullet);
            }
        }

        private Bullet CreateBullet() {
            Bullet bullet = Instantiate(
                bulletPrefab,
                transform
            );

            bullet.SetPoolManager(this);
            bullet.gameObject.SetActive(false);

            return bullet;
        }

        public void SpawnBullet(
            Vector2 spawnPosition,
            Vector2 targetPosition,
            BulletDataSO data,
            int bonusDamage = 0,
            float bonusKnockback = 0f
        ) {
            Bullet bullet = GetAvailableBullet();

            if (bullet == null) {
                Debug.LogWarning(
                    "사용 가능한 총알이 없고 풀 확장이 비활성화되어 있습니다.",
                    this
                );

                return;
            }

            bullet.transform.SetParent(transform);
            bullet.transform.position = spawnPosition;
            bullet.gameObject.SetActive(true);

            bullet.OnGetFromPool();

            bullet.Shoot(
                targetPosition,
                data,
                bonusDamage,
                bonusKnockback
            );
        }

        private Bullet GetAvailableBullet() {
            if (_bulletPool.Count > 0) {
                return _bulletPool.Dequeue();
            }

            if (canExpand) {
                return CreateBullet();
            }

            return null;
        }

        public void ReturnBullet(Bullet bullet) {
            if (bullet == null)
                return;

            bullet.OnReturnToPool();

            bullet.gameObject.SetActive(false);
            bullet.transform.SetParent(transform);

            _bulletPool.Enqueue(bullet);
        }
    }
}