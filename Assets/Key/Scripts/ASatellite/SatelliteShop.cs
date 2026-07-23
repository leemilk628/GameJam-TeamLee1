using System.Collections.Generic;
using UnityEngine;
using Key.Scripts.ASatellite.Modules;

namespace Key.Scripts.ASatellite {
    public class SatelliteShop : MonoBehaviour {
        [Header("Temporary Money")] 
        [SerializeField] private int _money = 100;

        [Header("Satellite Products")] [SerializeField]
        private List<ASatelliteSO> _products = new();

        [Header("Spawn Settings")] [SerializeField]
        private Transform _orbitCenter;

        [SerializeField] private Transform _satelliteParent;
        
        private readonly List<MovementModule> _satellites = new();

        public int Money => _money;

        public void BuySatellite(int productIndex) {
            if (productIndex < 0 || productIndex >= _products.Count) return;

            ASatelliteSO product = _products[productIndex];

            if ( product == null ||product.prefab == null || _orbitCenter == null) return;
            
            int price = product.price;

            if (_money < price) {
                // 구매 실패 팝업

                return;
            }

            _money -= price;

            AbstractASatellite satellite = Instantiate(
                product.prefab,
                _orbitCenter.position,
                Quaternion.identity,
                _satelliteParent
            );

            satellite.Deploy(_orbitCenter);

            MovementModule movementModule = satellite.GetModule<MovementModule>();

            if (movementModule != null) {
                _satellites.Add(movementModule);
                RearrangeSatellites();
            }
        }
        
        //위성 개수에 맞게 위치 조정하는 코드입니당
        private void RearrangeSatellites() {
            _satellites.RemoveAll(satellite => satellite == null);

            int satelliteCount = _satellites.Count;

            if (satelliteCount == 0)
                return;

            float baseAngle = _satellites[0].CurrentAngle;
            float angleGap = 360f / satelliteCount;

            for (int i = 0; i < satelliteCount; i++) {
                float angle = baseAngle + angleGap * i;

                _satellites[i].SetAngle(angle);
            }
        }

        public void AddMoney(int amount) {
            if (amount <= 0)
                return;

            _money += amount;
        }
    }
}