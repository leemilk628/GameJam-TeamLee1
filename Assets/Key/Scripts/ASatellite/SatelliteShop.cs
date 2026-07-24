using System.Collections.Generic;
using Eric.Currency;
using Eric.ModuleSystem;
using Eric.Satellite;
using Eric.ScriptableScripts;
using Eric.StageUpgrade;
using Key.Scripts.ASatellite.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace Key.Scripts.ASatellite {
    public class SatelliteShop : MonoBehaviour {
        [Header("Satellite Products")]
        [SerializeField] private List<ASatelliteSO> _products = new();

        [Tooltip("_products와 같은 순서로 설정")]
        [SerializeField] private List<SatelliteType> _productTypes = new();

        [Header("Spawn Settings")]
        [SerializeField] private Transform _orbitCenter;
        [SerializeField] private Transform _satelliteParent;

        private readonly List<MovementModule> _satellites = new();

        private GoldModule _goldModule;
        private SatelliteStageStatModule _satelliteStatModule;
        private SatelliteUnlockModule _satelliteUnlockModule;

        public int Money =>
            _goldModule != null
                ? _goldModule.CurrentGold
                : 0;

        public int CurrentSatelliteCount {
            get {
                RemoveMissingSatellites();
                return _satellites.Count;
            }
        }

        private void Start() {
            ConnectModules();
        }

        private void ConnectModules() {
            if (StageModuleOwner.Instance != null) {
                _goldModule =
                    StageModuleOwner.Instance
                        .GetModule<GoldModule>();

                _satelliteStatModule =
                    StageModuleOwner.Instance
                        .GetModule<SatelliteStageStatModule>();
            }

            if (GameModuleOwner.Instance != null) {
                _satelliteUnlockModule =
                    GameModuleOwner.Instance
                        .GetModule<SatelliteUnlockModule>();
            }

            if (_goldModule == null) {
                Debug.LogError(
                    $"{name}: GoldModule을 찾을 수 없습니다.",
                    this
                );
            }

            if (_satelliteStatModule == null) {
                Debug.LogError(
                    $"{name}: SatelliteStageStatModule을 찾을 수 없습니다.",
                    this
                );
            }

            if (_satelliteUnlockModule == null) {
                Debug.LogError(
                    $"{name}: SatelliteUnlockModule을 찾을 수 없습니다.",
                    this
                );
            }
        }

        public void BuySatellite(int productIndex) {
            if (!TryGetProduct(
                    productIndex,
                    out ASatelliteSO product,
                    out SatelliteType satelliteType
                )) {
                return;
            }

            if (_goldModule == null ||
                _satelliteStatModule == null ||
                _satelliteUnlockModule == null) {
                ConnectModules();
            }

            if (_goldModule == null ||
                _satelliteStatModule == null) {
                return;
            }

            if (_orbitCenter == null ||
                _satelliteParent == null) {
                Debug.LogError(
                    $"{name}: 위성 생성 위치가 설정되지 않았습니다.",
                    this
                );

                return;
            }

            if (product.prefab == null) {
                Debug.LogError(
                    $"{product.name}: 위성 프리팹이 없습니다.",
                    product
                );

                return;
            }

            RemoveMissingSatellites();

            if (!_satelliteStatModule.CanSpawnSatellite(
                    _satellites.Count
                )) {
                // 최대 위성 개수 초과 팝업
                return;
            }

            if (satelliteType != SatelliteType.None) {
                if (_satelliteUnlockModule == null)
                    return;

                if (!_satelliteUnlockModule.IsUnlocked(
                        satelliteType
                    )) {
                    // 아직 해금되지 않은 위성 팝업
                    return;
                }
            }

            if (!_goldModule.TrySpendGold(product.price)) {
                // 골드 부족 팝업
                return;
            }

            AbstractASatellite satellite = Instantiate(
                product.prefab,
                _orbitCenter.position,
                Quaternion.identity,
                _satelliteParent
            );

            MovementModule movementModule =
                satellite.GetModule<MovementModule>();

            if (movementModule == null) {
                Debug.LogError(
                    $"{satellite.name}: MovementModule이 없습니다.",
                    satellite
                );

                RefundGold(product.price);
                Destroy(satellite.gameObject);
                return;
            }

            satellite.Deploy(_orbitCenter);

            _satellites.Add(movementModule);
            RearrangeSatellites();
        }

        private bool TryGetProduct(
            int productIndex,
            out ASatelliteSO product,
            out SatelliteType satelliteType
        ) {
            product = null;
            satelliteType = SatelliteType.None;

            if (productIndex < 0 ||
                productIndex >= _products.Count) {
                Debug.LogError(
                    $"{name}: 잘못된 상품 인덱스입니다. " +
                    $"Index: {productIndex}",
                    this
                );

                return false;
            }

            product = _products[productIndex];

            if (product == null) {
                Debug.LogError(
                    $"{name}: 상품 데이터가 비어 있습니다.",
                    this
                );

                return false;
            }

            if (productIndex < _productTypes.Count) {
                satelliteType =
                    _productTypes[productIndex];
            }

            return true;
        }

        private void RearrangeSatellites() {
            RemoveMissingSatellites();

            int satelliteCount =
                _satellites.Count;

            if (satelliteCount == 0)
                return;

            float baseAngle =
                _satellites[0].CurrentAngle;

            float angleGap =
                360f / satelliteCount;

            for (int i = 0; i < satelliteCount; i++) {
                float angle =
                    baseAngle + angleGap * i;

                _satellites[i].SetAngle(angle);
            }
        }

        private void RemoveMissingSatellites() {
            _satellites.RemoveAll(
                satellite => satellite == null
            );
        }

        private void RefundGold(int amount) {
            if (_goldModule == null ||
                amount <= 0) {
                return;
            }

            _goldModule.SetGold(
                _goldModule.CurrentGold + amount
            );
        }

        public void AddMoney(int amount) {
            if (amount <= 0)
                return;

            if (_goldModule == null)
                ConnectModules();

            _goldModule?.AddGold(amount);
        }

        public bool CanBuySatellite(int productIndex) {
            if (!TryGetProduct(
                    productIndex,
                    out ASatelliteSO product,
                    out SatelliteType satelliteType
                )) {
                return false;
            }

            if (_goldModule == null ||
                _satelliteStatModule == null ||
                _satelliteUnlockModule == null) {
                ConnectModules();
            }

            if (_goldModule == null ||
                _satelliteStatModule == null) {
                return false;
            }

            RemoveMissingSatellites();

            if (!_satelliteStatModule.CanSpawnSatellite(
                    _satellites.Count
                )) {
                return false;
            }

            if (satelliteType != SatelliteType.None) {
                if (_satelliteUnlockModule == null ||
                    !_satelliteUnlockModule.IsUnlocked(
                        satelliteType
                    )) {
                    return false;
                }
            }

            return _goldModule.HasGold(product.price);
        }

        public void ButtonLock(Button button) {
            if (button == null)
                return;

            if (_satelliteStatModule == null)
                ConnectModules();

            RemoveMissingSatellites();

            button.interactable =
                _satelliteStatModule != null &&
                _satelliteStatModule.CanSpawnSatellite(
                    _satellites.Count
                );
        }
    }
}