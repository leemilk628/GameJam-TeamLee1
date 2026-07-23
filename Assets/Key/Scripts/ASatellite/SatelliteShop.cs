using System;
using System.Collections.Generic;
using UnityEngine;

namespace Key.Scripts.ASatellite
{
    public class SatelliteShop : MonoBehaviour
    {
        [Serializable]
        private class SatelliteProduct
        {
            [SerializeField] private string productName;
            [SerializeField] private AbstractASatellite prefab;
            [SerializeField, Min(0)] private int price;

            public string ProductName => productName;
            public AbstractASatellite Prefab => prefab;
            public int Price => price;
        }

        [Header("Temporary Money")]
        [SerializeField] private int _money = 100;

        [Header("Satellite Products")]
        [SerializeField] private List<SatelliteProduct> _products = new();

        [Header("Spawn Settings")]
        [SerializeField] private Transform _orbitCenter;
        [SerializeField] private Transform _satelliteParent;

        public int Money => _money;

        // UI 버튼의 OnClick에서 상품 번호를 전달
        public void BuySatellite(int productIndex)
        {
            if (productIndex < 0 || productIndex >= _products.Count)
            {
                Debug.LogError(
                    $"잘못된 상품 번호입니다: {productIndex}",
                    this
                );

                return;
            }

            SatelliteProduct product = _products[productIndex];

            if (product.Prefab == null)
            {
                Debug.LogError(
                    $"{product.ProductName}의 프리팹이 설정되지 않았습니다.",
                    this
                );

                return;
            }

            if (_orbitCenter == null)
            {
                Debug.LogError(
                    "Orbit Center가 설정되지 않았습니다.",
                    this
                );

                return;
            }

            if (_money < product.Price)
            {
                Debug.Log(
                    $"{product.ProductName} 구매 실패: " +
                    $"돈이 부족합니다. 현재 재화: {_money}, 가격: {product.Price}",
                    this
                );

                return;
            }

            // 재화 차감
            _money -= product.Price;

            // 인공위성 생성
            AbstractASatellite satellite = Instantiate(
                product.Prefab,
                _orbitCenter.position,
                Quaternion.identity,
                _satelliteParent
            );

            // 원운동 중심과 공격 모듈 설정
            satellite.Deploy(_orbitCenter);

            Debug.Log(
                $"{product.ProductName} 구매 완료. " +
                $"남은 재화: {_money}",
                this
            );
        }

        // 임시 테스트용 재화 추가
        public void AddMoney(int amount)
        {
            if (amount <= 0)
                return;

            _money += amount;

            Debug.Log($"재화 추가: {amount}, 현재 재화: {_money}", this);
        }
    }
}