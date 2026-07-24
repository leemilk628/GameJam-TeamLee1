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
        private SatelliteStageStatModule
            _subscribedSatelliteStatModule;

        private int _currentProductIndex = -1;

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
            SelectInitialMode();
            SynchronizeSatelliteCount();
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

            SubscribeToSatelliteStats();
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

            if (_currentProductIndex == productIndex)
                return;

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

            _currentProductIndex = productIndex;

            ReplaceSatelliteMode(product);
            SynchronizeSatelliteCount();
        }

        private bool SpawnSatellite(
            ASatelliteSO product,
            float? angle = null
        ) {
            if (product == null ||
                product.prefab == null ||
                _orbitCenter == null ||
                _satelliteParent == null) {
                return false;
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

                Destroy(satellite.gameObject);
                return false;
            }

            satellite.Deploy(_orbitCenter);

            if (angle.HasValue)
                movementModule.SetAngle(angle.Value);

            _satellites.Add(movementModule);
            return true;
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

        private void SubscribeToSatelliteStats() {
            if (_subscribedSatelliteStatModule ==
                _satelliteStatModule) {
                return;
            }

            if (_subscribedSatelliteStatModule != null) {
                _subscribedSatelliteStatModule.OnStatsChanged -=
                    HandleSatelliteStatsChanged;
            }

            _subscribedSatelliteStatModule =
                _satelliteStatModule;

            if (_subscribedSatelliteStatModule != null) {
                _subscribedSatelliteStatModule.OnStatsChanged +=
                    HandleSatelliteStatsChanged;
            }
        }

        private void HandleSatelliteStatsChanged() {
            SynchronizeSatelliteCount();
        }

        private void SelectInitialMode() {
            if (_currentProductIndex >= 0 &&
                _currentProductIndex < _products.Count &&
                _products[_currentProductIndex] != null) {
                return;
            }

            for (int i = 0; i < _products.Count; i++) {
                ASatelliteSO product = _products[i];

                if (product == null ||
                    product.prefab == null) {
                    continue;
                }

                SatelliteType satelliteType =
                    i < _productTypes.Count
                        ? _productTypes[i]
                        : SatelliteType.None;

                if (!IsUnlocked(satelliteType))
                    continue;

                _currentProductIndex = i;
                return;
            }
        }

        private bool IsUnlocked(
            SatelliteType satelliteType
        ) {
            if (satelliteType == SatelliteType.None)
                return true;

            return _satelliteUnlockModule != null &&
                   _satelliteUnlockModule.IsUnlocked(
                       satelliteType
                   );
        }

        private void SynchronizeSatelliteCount() {
            if (_satelliteStatModule == null)
                ConnectModules();

            if (_satelliteStatModule == null)
                return;

            SelectInitialMode();

            if (!TryGetProduct(
                    _currentProductIndex,
                    out ASatelliteSO product,
                    out _
                )) {
                return;
            }

            if (_orbitCenter == null ||
                _satelliteParent == null ||
                product.prefab == null) {
                return;
            }

            RemoveMissingSatellites();

            int targetCount =
                _satelliteStatModule.MaxSatelliteCount;

            while (_satellites.Count < targetCount) {
                if (!SpawnSatellite(product))
                    break;
            }

            while (_satellites.Count > targetCount) {
                int lastIndex =
                    _satellites.Count - 1;

                MovementModule movementModule =
                    _satellites[lastIndex];

                _satellites.RemoveAt(lastIndex);
                DestroySatellite(movementModule);
            }

            RearrangeSatellites();
        }

        private void ReplaceSatelliteMode(
            ASatelliteSO product
        ) {
            RemoveMissingSatellites();

            List<float> angles = new(
                _satellites.Count
            );

            foreach (MovementModule satellite in _satellites)
                angles.Add(satellite.CurrentAngle);

            foreach (MovementModule satellite in _satellites)
                DestroySatellite(satellite);

            _satellites.Clear();

            foreach (float angle in angles) {
                if (!SpawnSatellite(product, angle))
                    break;
            }

            RearrangeSatellites();
        }

        private void DestroySatellite(
            MovementModule movementModule
        ) {
            if (movementModule == null)
                return;

            AbstractASatellite satellite =
                movementModule
                    .GetComponentInParent<AbstractASatellite>();

            if (satellite != null)
                Destroy(satellite.gameObject);
            else
                Destroy(movementModule.gameObject);
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

            if (_currentProductIndex == productIndex)
                return false;

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
                _satelliteStatModule != null;
        }

        private void OnDestroy() {
            if (_subscribedSatelliteStatModule != null) {
                _subscribedSatelliteStatModule.OnStatsChanged -=
                    HandleSatelliteStatsChanged;
            }
        }
    }
}
