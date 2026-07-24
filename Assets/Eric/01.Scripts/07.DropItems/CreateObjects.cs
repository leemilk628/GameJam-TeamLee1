using System;
using System.Collections.Generic;
using Eric.Currency;
using Eric.DropItem;
using UnityEngine;

namespace Eric.DropItems
{
    public enum DropItemType
    {
        MeteoriteFragment,
        Gold
    }
    public class CreateObjects : MonoBehaviour
    {
        private readonly struct DropData
        {
            public readonly DropItemType Type;
            public readonly int Amount;

            public DropData(DropItemType type, int amount)
            {
                Type = type;
                Amount = amount;
            }
        }

        public static CreateObjects Instance { get; private set; }

        [Header("드롭 프리팹")]
        public SelectImageSO meteoSo;
        public SelectImageSO goldSo;

        [Header("호출 1회당 연출 설정")]
        [SerializeField, Min(2)] private int maxObjectsPerRequest = 10;
        [SerializeField, Min(0f)] private float itemStartInterval = 0.05f;

        [Header("재화 모듈")]
        [SerializeField] private GoldModule goldModule = null;
        [SerializeField] private MeteoriteFragmentModule meteoriteFragmentModule = null;

        public event Action<int, int, Vector3> HandleCreateObject;

        public Stack<GameObject> meteoPrefabs = new();
        public Stack<GameObject> goldPrefabs = new();

        private readonly HashSet<GameObject> pooledObjects = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                enabled = false;
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            if (Instance == null) Instance = this;
            if (Instance == this) HandleCreateObject += CreateObject;
        }

        private void OnDisable()
        {
            HandleCreateObject -= CreateObject;
            if (Instance == this) Instance = null;
        }

        public void CreateEvent(int mfValue, int gValue, Vector3 position)
        {
            HandleCreateObject?.Invoke(mfValue, gValue, position);
        }

        private void CreateObject(int mfValue, int gValue, Vector3 position)
        {
            int meteoriteBase = Mathf.Max(0, mfValue);
            int goldBase = Mathf.Max(0, gValue);

            if (meteoriteBase == 0 && goldBase == 0) return;
            if (meteoriteBase > 0 && meteoriteFragmentModule == null) return;
            if (goldBase > 0 && goldModule == null) return;

            int visualCount = (int)Math.Min((long)meteoriteBase + goldBase, maxObjectsPerRequest);
            (int meteoriteCount, int goldCount) = GetVisualCounts(
                meteoriteBase,
                goldBase,
                visualCount
            );

            int finalMeteorite = meteoriteCount == 0
                ? 0
                : meteoriteFragmentModule.CalculateFinalMeteoriteFragmentAmount(meteoriteBase);

            int finalGold = goldCount == 0
                ? 0
                : goldModule.CalculateFinalGoldAmount(goldBase);

            int[] meteoriteRewards = SplitReward(finalMeteorite, meteoriteCount);
            int[] goldRewards = SplitReward(finalGold, goldCount);

            List<DropData> plan = MixRewards(
                meteoriteRewards,
                goldRewards
            );

            for (int i = 0; i < plan.Count; i++)
                Spawn(plan[i], position, i * itemStartInterval);
        }

        private static (int meteorite, int gold) GetVisualCounts(
            int meteoriteBase,
            int goldBase,
            int total)
        {
            if (meteoriteBase == 0) return (0, total);
            if (goldBase == 0) return (total, 0);

            double ratio = meteoriteBase / ((double)meteoriteBase + goldBase);

            int meteorite = Mathf.Clamp(
                (int)Math.Round(
                    total * ratio,
                    MidpointRounding.AwayFromZero
                ),
                1,
                total - 1
            );

            return (meteorite, total - meteorite);
        }

        private static int[] SplitReward(int totalReward, int count)
        {
            if (count == 0) return Array.Empty<int>();

            int[] result = new int[count];
            long previous = 0;

            for (int i = 0; i < count; i++)
            {
                long current = (long)(i + 1) * totalReward / count;
                result[i] = (int)(current - previous);
                previous = current;
            }

            return result;
        }

        private static List<DropData> MixRewards(
            int[] meteoriteRewards,
            int[] goldRewards)
        {
            int meteoriteIndex = 0;
            int goldIndex = 0;
            int total = meteoriteRewards.Length + goldRewards.Length;

            List<DropData> result = new(total);

            for (int i = 0; i < total; i++)
            {
                int targetGold = (int)Math.Floor(
                    (i + 1) *
                    goldRewards.Length /
                    (double)total +
                    0.5d
                );

                bool useGold =
                    goldIndex < targetGold &&
                    goldIndex < goldRewards.Length;

                if (useGold)
                {
                    result.Add(
                        new DropData(
                            DropItemType.Gold,
                            goldRewards[goldIndex++]
                        )
                    );
                }
                else
                {
                    result.Add(
                        new DropData(
                            DropItemType.MeteoriteFragment,
                            meteoriteRewards[meteoriteIndex++]
                        )
                    );
                }
            }

            return result;
        }

        private void Spawn(
            DropData data,
            Vector3 position,
            float delay)
        {
            GameObject itemObject = GetObject(data.Type, position);

            if (itemObject == null)
            {
                GrantReward(data.Type, data.Amount);
                return;
            }

            if (!itemObject.TryGetComponent(out ItemGetEffect effect))
            {
                GrantReward(data.Type, data.Amount);
                itemObject.SetActive(false);
                ReturnToPool(data.Type, itemObject);
                return;
            }

            itemObject.transform.SetPositionAndRotation(
                position,
                Quaternion.identity
            );

            effect.Initialize(
                data.Type,
                data.Amount,
                delay,
                OnCollected,
                OnItemDisabled
            );
        }

        private GameObject GetObject(
            DropItemType type,
            Vector3 position)
        {
            Stack<GameObject> pool = GetPool(type);

            while (pool.Count > 0)
            {
                GameObject itemObject = pool.Pop();

                if (itemObject == null) continue;

                pooledObjects.Remove(itemObject);
                return itemObject;
            }

            SelectImageSO data =
                type == DropItemType.MeteoriteFragment
                    ? meteoSo
                    : goldSo;

            GameObject prefab =
                data == null
                    ? null
                    : data.SelectedObject;

            return prefab == null
                ? null
                : Instantiate(
                    prefab,
                    position,
                    Quaternion.identity
                );
        }

        private void OnCollected(ItemGetEffect effect)
        {
            if (effect == null) return;

            GrantReward(
                effect.ItemType,
                effect.RewardAmount
            );

            effect.gameObject.SetActive(false);
        }

        private void OnItemDisabled(ItemGetEffect effect)
        {
            if (effect != null)
            {
                ReturnToPool(
                    effect.ItemType,
                    effect.gameObject
                );
            }
        }

        private void GrantReward(
            DropItemType type,
            int amount)
        {
            if (amount <= 0) return;

            if (type == DropItemType.MeteoriteFragment)
                meteoriteFragmentModule.AddCalculatedMeteoriteFragment(amount);
            else
                goldModule.AddCalculatedGold(amount);
        }

        private void ReturnToPool(
            DropItemType type,
            GameObject itemObject)
        {
            if (itemObject != null &&
                pooledObjects.Add(itemObject))
            {
                GetPool(type).Push(itemObject);
            }
        }

        private Stack<GameObject> GetPool(DropItemType type)
        {
            return type == DropItemType.MeteoriteFragment
                ? meteoPrefabs
                : goldPrefabs;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            maxObjectsPerRequest = Mathf.Max(2, maxObjectsPerRequest);
            itemStartInterval = Mathf.Max(0f, itemStartInterval);
        }
#endif
    }
}