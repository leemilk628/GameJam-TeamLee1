using System.Collections.Generic;
using Key.Scripts.BulletSc;
using Key.Scripts.Projectile;
using UnityEngine;

namespace Key.Scripts.Pooling

{
    public class BulletPoolManager : MonoBehaviour {
        [Header("Pool")] [SerializeField] private Bullet bulletPrefab;

        [Min(1)] [SerializeField] private int initialPoolSize = 30;

        [SerializeField] private bool canExpand = true;

        private readonly Queue<Bullet> _bulletPool = new();

        private void Awake() {
            CreateInitialPool();
        }

        private void CreateInitialPool() {
            if (bulletPrefab == null) {
                Debug.LogError(
                    "BulletPoolManager에 Bullet Prefab이 설정되지 않았습니다."
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

        public Bullet SpawnBullet(Vector3 spawnPosition, Vector2 targetPosition,
            BulletDataSO bulletData, int bonusDamage = 0, float bonusKnockback = 0f) {
            Bullet bullet = GetAvailableBullet();

            if (bullet == null)
                return null;

            bullet.transform.SetParent(null);

            bullet.transform.SetPositionAndRotation(
                spawnPosition,
                Quaternion.identity
            );

            bullet.gameObject.SetActive(true);
            bullet.OnGetFromPool();

            bullet.Shoot(
                targetPosition,
                bulletData,
                bonusDamage,
                bonusKnockback
            );

            return bullet;
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