using System.Collections.Generic;
using Key.Scripts.BulletSc;
using Key.Scripts.Projectile;
using UnityEngine;

namespace Key.Scripts.Pooling {
    public class BulletPoolManager : MonoBehaviour {
        [Header("Pool")]
        [Min(1)]
        [SerializeField] private int initialPoolSize = 10;

        [SerializeField] private bool canExpand = true;

        private readonly Dictionary<Bullet, Queue<Bullet>> _bulletPools = new();
        private readonly Dictionary<Bullet, Bullet> _bulletPrefabByInstance = new();

        public void SpawnBullet(
            Bullet bulletPrefab,
            Vector2 spawnPosition,
            Vector2 targetPosition,
            BulletDataSO data,
            int bonusDamage = 0,
            float bonusKnockback = 0f,
            float? rotationOffsetOverride = null
        ) {
            if (bulletPrefab == null) {
                return;
            }

            if (data == null) {
                return;
            }

            Bullet bullet = GetAvailableBullet(bulletPrefab);

            if (bullet == null)
                return;

            bullet.transform.SetParent(transform);
            bullet.transform.position = spawnPosition;
            bullet.gameObject.SetActive(true);

            bullet.OnGetFromPool();

            bullet.Shoot(
                targetPosition,
                data,
                bonusDamage,
                bonusKnockback,
                rotationOffsetOverride
            );
        }

        private Bullet GetAvailableBullet(Bullet bulletPrefab) {
            Queue<Bullet> bulletPool =
                GetOrCreatePool(bulletPrefab);

            if (bulletPool.Count > 0)
                return bulletPool.Dequeue();

            if (canExpand)
                return CreateBullet(bulletPrefab);

            return null;
        }

        private Queue<Bullet> GetOrCreatePool(Bullet bulletPrefab) {
            if (_bulletPools.TryGetValue(
                    bulletPrefab,
                    out Queue<Bullet> bulletPool
                )) {
                return bulletPool;
            }

            bulletPool = new Queue<Bullet>();
            _bulletPools.Add(bulletPrefab, bulletPool);

            for (int i = 0; i < initialPoolSize; i++) {
                Bullet bullet = CreateBullet(bulletPrefab);
                bulletPool.Enqueue(bullet);
            }

            return bulletPool;
        }

        private Bullet CreateBullet(Bullet bulletPrefab) {
            Bullet bullet = Instantiate(
                bulletPrefab,
                transform
            );

            bullet.SetPoolManager(this);
            bullet.gameObject.SetActive(false);

            _bulletPrefabByInstance.Add(
                bullet,
                bulletPrefab
            );

            return bullet;
        }

        public void ReturnBullet(Bullet bullet) {
            if (bullet == null)
                return;

            if (!_bulletPrefabByInstance.TryGetValue(bullet, out Bullet bulletPrefab)) {
                Destroy(bullet.gameObject);
                return;
            }

            Queue<Bullet> bulletPool =
                GetOrCreatePool(bulletPrefab);

            bullet.OnReturnToPool();

            bullet.gameObject.SetActive(false);
            bullet.transform.SetParent(transform);

            bulletPool.Enqueue(bullet);
        }
    }
}
