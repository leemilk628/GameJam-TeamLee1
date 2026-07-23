using System;
using System.Collections.Generic;
using UnityEngine;

namespace Key.Scripts.ASatellite {
    [Serializable]
    public class BuyASatellite : MonoBehaviour {
        public string productName;
        public AbstractASatellite prefab;

        [Min(0)] public int price;


        [Header("Products")] [SerializeField] private List<SatelliteProduct> products = new();

        [Header("Spawn")] [SerializeField] private Transform orbitCenter;
        [SerializeField] private Transform satelliteParent;

        private int _money;

        public void BuySatellite(int productIndex) {
            if (productIndex < 0 ||
                productIndex >= products.Count) {
                Debug.LogError(
                    $"존재하지 않는 상품 번호입니다: {productIndex}",
                    this
                );

                return;
            }

            SatelliteProduct product =
                products[productIndex];

            if (product.prefab == null) {
                Debug.LogError(
                    $"{product.productName}의 프리팹이 없습니다.",
                    this
                );

                return;
            }

            if (orbitCenter == null) {
                Debug.LogError(
                    "Orbit Center가 설정되지 않았습니다.",
                    this
                );

                return;
            }

            if (wallet == null) {
                Debug.LogError(
                    "MoneyWallet이 설정되지 않았습니다.",
                    this
                );

                return;
            }

            // 돈이 부족하면 구매 실패
            if (!wallet.TrySpend(product.price)) {
                Debug.Log(
                    $"{product.productName} 구매 실패: 돈이 부족합니다."
                );

                return;
            }

            AbstractASatellite satellite = Instantiate(
                product.prefab,
                orbitCenter.position,
                Quaternion.identity,
                satelliteParent
            );

            satellite.Deploy(orbitCenter);

            Debug.Log(
                $"{product.productName} 구매 완료. 가격: {product.price}"
            );
        }
    }
}