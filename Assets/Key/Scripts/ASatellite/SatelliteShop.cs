using System.Collections.Generic;
using Eric.Currency;
using Eric.ModuleSystem;
using Eric.Satellite;
using Eric.ScriptableScripts;
using Eric.StageUpgrade;
using Key.Scripts.ASatellite.Modules;
using Key.Scripts.Player;
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
        private SatelliteUnlockModule
            _subscribedSatelliteUnlockModule;

        private readonly List<Button> _productButtons = new();

        private int _currentProductIndex = -1;
        private AbstractASatellite _activeSatelliteTemplate;
        private AbstractASatellite _basicSatelliteTemplate;

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
            ResolveSpawnSettings();
            ConnectModules();
            RegisterExistingSatellites();
            FindProductButtons();
            RefreshProductButtons();
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
            SubscribeToSatelliteUnlock();
        }

        private void ResolveSpawnSettings() {
            if (_orbitCenter == null) {
                EarthHealth earthHealth =
                    FindFirstObjectByType<EarthHealth>();

                if (earthHealth != null) {
                    _orbitCenter =
                        earthHealth.transform;
                }
                else {
                    PlayerStat playerStat =
                        FindFirstObjectByType<PlayerStat>();

                    if (playerStat != null) {
                        _orbitCenter =
                            playerStat.transform.root;
                    }
                }
            }

            if (_satelliteParent != null)
                return;

            GameObject satelliteParent =
                GameObject.Find("Satellites");

            if (satelliteParent == null) {
                satelliteParent =
                    new GameObject("Satellites");
            }

            _satelliteParent =
                satelliteParent.transform;
        }

        public void ChangeSatellite(int productIndex) {
            if (!TryGetProduct(
                    productIndex,
                    out ASatelliteSO product,
                    out SatelliteType satelliteType
                )) {
                return;
            }

            if (_satelliteStatModule == null ||
                _satelliteUnlockModule == null) {
                ConnectModules();
            }

            if (_satelliteStatModule == null) {
                return;
            }

            if (_orbitCenter == null ||
                _satelliteParent == null) {
                ResolveSpawnSettings();
            }

            if (_orbitCenter == null ||
                _satelliteParent == null) {
                    Debug.LogError(
                    $"{name}: 위성 생성 위치가 설정되지 않았습니다.",
                    this
                );

                return;
            }

            AbstractASatellite satellitePrefab =
                GetProductPrefab(
                    productIndex,
                    product,
                    satelliteType
                );

            if (satellitePrefab == null) {
                Debug.LogError(
                    $"{product.name}: 위성 프리팹이 없습니다.",
                    product
                );

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

            _currentProductIndex = productIndex;

            ReplaceSatelliteMode(satellitePrefab);
            SynchronizeSatelliteCount();
        }

        private bool SpawnSatellite(
            ASatelliteSO product,
            float? angle = null
        ) {
            if (product == null)
                return false;

            return SpawnSatellite(product.prefab, angle);
        }

        private bool SpawnSatellite(
            AbstractASatellite satellitePrefab,
            float? angle = null
        ) {
            if (satellitePrefab == null ||
                _orbitCenter == null ||
                _satelliteParent == null) {
                return false;
            }

            AbstractASatellite satellite = Instantiate(
                satellitePrefab,
                _orbitCenter.position,
                Quaternion.identity,
                _satelliteParent
            );

            if (!satellite.gameObject.activeSelf)
                satellite.gameObject.SetActive(true);

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

            satelliteType = GetProductType(productIndex);

            return true;
        }

        private SatelliteType GetProductType(
            int productIndex
        ) {
            if (productIndex >= 0 &&
                productIndex < _productTypes.Count &&
                _productTypes[productIndex] !=
                SatelliteType.None) {
                return _productTypes[productIndex];
            }

            return productIndex switch {
                0 => SatelliteType.AutocannonSatellite,
                1 => SatelliteType.MissileSatellite,
                2 => SatelliteType.LaserSatellite,
                _ => SatelliteType.None
            };
        }

        private AbstractASatellite GetProductPrefab(
            int productIndex,
            ASatelliteSO product,
            SatelliteType satelliteType
        ) {
            if (satelliteType == SatelliteType.None &&
                _basicSatelliteTemplate != null) {
                return _basicSatelliteTemplate;
            }

            if (productIndex < 0 ||
                productIndex >= _products.Count ||
                product == null) {
                return null;
            }

            return product.prefab;
        }

        private void RegisterExistingSatellites() {
            AbstractASatellite[] existingSatellites =
                FindObjectsByType<AbstractASatellite>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                );

            foreach (AbstractASatellite satellite in
                     existingSatellites) {
                if (satellite == null) {
                    continue;
                }

                MovementModule movementModule =
                    satellite.GetModule<MovementModule>();

                if (movementModule == null ||
                    _satellites.Contains(movementModule)) {
                    continue;
                }

                if (_basicSatelliteTemplate == null) {
                    _basicSatelliteTemplate =
                        Instantiate(
                            satellite,
                            transform
                        );

                    _basicSatelliteTemplate.name =
                        $"{satellite.name} Template";

                    _basicSatelliteTemplate
                        .gameObject
                        .SetActive(false);
                }

                satellite.Deploy(_orbitCenter);
                _satellites.Add(movementModule);

                if (_activeSatelliteTemplate == null)
                    _activeSatelliteTemplate = satellite;
            }

            RearrangeSatellites();
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

        private void SubscribeToSatelliteUnlock() {
            if (_subscribedSatelliteUnlockModule ==
                _satelliteUnlockModule) {
                return;
            }

            if (_subscribedSatelliteUnlockModule != null) {
                _subscribedSatelliteUnlockModule
                    .OnSatelliteUnlockChanged -=
                    HandleSatelliteUnlockChanged;
            }

            _subscribedSatelliteUnlockModule =
                _satelliteUnlockModule;

            if (_subscribedSatelliteUnlockModule != null) {
                _subscribedSatelliteUnlockModule
                    .OnSatelliteUnlockChanged +=
                    HandleSatelliteUnlockChanged;
            }
        }

        private void HandleSatelliteUnlockChanged() {
            RefreshProductButtons();
            SelectInitialMode();
            SynchronizeSatelliteCount();
        }

        private void SelectInitialMode() {
            if (_currentProductIndex >= 0 &&
                _currentProductIndex < _products.Count &&
                _products[_currentProductIndex] != null) {
                return;
            }

            RemoveMissingSatellites();

            if (_satellites.Count > 0 &&
                _activeSatelliteTemplate != null) {
                return;
            }

            for (int i = 0; i < _products.Count; i++) {
                ASatelliteSO product = _products[i];

                if (product == null ||
                    product.prefab == null) {
                    continue;
                }

                SatelliteType satelliteType =
                    GetProductType(i);

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
            if (_orbitCenter == null ||
                _satelliteParent == null) {
                ResolveSpawnSettings();
            }

            if (_satelliteStatModule == null)
                ConnectModules();

            if (_satelliteStatModule == null)
                return;

            RegisterExistingSatellites();
            SelectInitialMode();

            AbstractASatellite satellitePrefab =
                GetActiveSatellitePrefab();

            if (_orbitCenter == null ||
                _satelliteParent == null ||
                satellitePrefab == null) {
                return;
            }

            RemoveMissingSatellites();

            int targetCount =
                _satelliteStatModule.MaxSatelliteCount;

            while (_satellites.Count < targetCount) {
                if (!SpawnSatellite(satellitePrefab))
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

        private AbstractASatellite GetActiveSatellitePrefab() {
            if (_currentProductIndex >= 0 &&
                _currentProductIndex < _products.Count) {
                ASatelliteSO product =
                    _products[_currentProductIndex];

                SatelliteType satelliteType =
                    GetProductType(_currentProductIndex);

                AbstractASatellite productPrefab =
                    GetProductPrefab(
                        _currentProductIndex,
                        product,
                        satelliteType
                    );

                if (productPrefab != null) {
                    _activeSatelliteTemplate =
                        productPrefab;

                    return productPrefab;
                }
            }

            if (_activeSatelliteTemplate != null)
                return _activeSatelliteTemplate;

            RemoveMissingSatellites();

            if (_satellites.Count == 0)
                return null;

            _activeSatelliteTemplate =
                _satellites[0]
                    .GetComponentInParent<AbstractASatellite>();

            return _activeSatelliteTemplate;
        }

        private void ReplaceSatelliteMode(
            AbstractASatellite satellitePrefab
        ) {
            if (satellitePrefab == null) {
                return;
            }

            _activeSatelliteTemplate = satellitePrefab;

            List<AbstractASatellite> activeSatellites =
                FindActiveSceneSatellites();

            List<float> angles = new(
                activeSatellites.Count
            );

            foreach (AbstractASatellite satellite in
                     activeSatellites) {
                MovementModule movementModule =
                    satellite.GetModule<MovementModule>();

                if (movementModule != null)
                    angles.Add(movementModule.CurrentAngle);

                satellite.gameObject.SetActive(false);
                Destroy(satellite.gameObject);
            }

            _satellites.Clear();

            foreach (float angle in angles) {
                if (!SpawnSatellite(
                        satellitePrefab,
                        angle
                    )) {
                    break;
                }
            }

            RearrangeSatellites();
        }

        private List<AbstractASatellite>
            FindActiveSceneSatellites() {
            AbstractASatellite[] foundSatellites =
                FindObjectsByType<AbstractASatellite>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                );

            List<AbstractASatellite> sceneSatellites =
                new(foundSatellites.Length);

            foreach (AbstractASatellite satellite in
                     foundSatellites) {
                if (satellite == null ||
                    satellite == _basicSatelliteTemplate) {
                    continue;
                }

                sceneSatellites.Add(satellite);
            }

            return sceneSatellites;
        }

        private void DestroySatellite(
            MovementModule movementModule
        ) {
            if (movementModule == null)
                return;

            AbstractASatellite satellite =
                movementModule
                    .GetComponentInParent<AbstractASatellite>();

            if (satellite != null) {
                satellite.gameObject.SetActive(false);
                Destroy(satellite.gameObject);
            }
            else {
                movementModule.gameObject.SetActive(false);
                Destroy(movementModule.gameObject);
            }
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

            if (_satelliteStatModule == null ||
                _satelliteUnlockModule == null) {
                ConnectModules();
            }

            if (_satelliteStatModule == null) {
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

            return true;
        }

        private void FindProductButtons() {
            _productButtons.Clear();

            Button[] buttons =
                FindObjectsByType<Button>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                );

            foreach (Button button in buttons) {
                if (!CallsBuySatellite(button))
                    continue;

                _productButtons.Add(button);
            }

            _productButtons.Sort(CompareHierarchyOrder);
        }

        private bool CallsBuySatellite(Button button) {
            if (button == null)
                return false;

            int listenerCount =
                button.onClick.GetPersistentEventCount();

            for (int i = 0; i < listenerCount; i++) {
                if (button.onClick.GetPersistentTarget(i) != this)
                    continue;

                if (button.onClick.GetPersistentMethodName(i) ==
                    nameof(ChangeSatellite)) {
                    return true;
                }
            }

            return false;
        }

        private static int CompareHierarchyOrder(
            Button left,
            Button right
        ) {
            List<Transform> leftPath =
                GetHierarchyPath(left.transform);

            List<Transform> rightPath =
                GetHierarchyPath(right.transform);

            int sharedDepth =
                Mathf.Min(leftPath.Count, rightPath.Count);

            for (int i = 0; i < sharedDepth; i++) {
                int comparison =
                    leftPath[i]
                        .GetSiblingIndex()
                        .CompareTo(
                            rightPath[i].GetSiblingIndex()
                        );

                if (comparison != 0)
                    return comparison;
            }

            return leftPath.Count.CompareTo(rightPath.Count);
        }

        private static List<Transform> GetHierarchyPath(
            Transform current
        ) {
            List<Transform> path = new();

            while (current != null) {
                path.Add(current);
                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        private void RefreshProductButtons() {
            if (_productButtons.Count == 0)
                FindProductButtons();

            int buttonCount =
                Mathf.Min(
                    _productButtons.Count,
                    _products.Count
                );

            for (int i = 0; i < buttonCount; i++) {
                Button button = _productButtons[i];

                if (button == null)
                    continue;

                button.interactable =
                    IsUnlocked(GetProductType(i));
            }
        }

        public void ButtonLock(Button button) {
            if (button == null)
                return;

            if (_satelliteUnlockModule == null)
                ConnectModules();

            if (!_productButtons.Contains(button)) {
                _productButtons.Add(button);
                _productButtons.Sort(CompareHierarchyOrder);
            }

            RefreshProductButtons();
        }

        private void OnDestroy() {
            if (_subscribedSatelliteStatModule != null) {
                _subscribedSatelliteStatModule.OnStatsChanged -=
                    HandleSatelliteStatsChanged;
            }

            if (_subscribedSatelliteUnlockModule != null) {
                _subscribedSatelliteUnlockModule
                    .OnSatelliteUnlockChanged -=
                    HandleSatelliteUnlockChanged;
            }
        }
    }
}
