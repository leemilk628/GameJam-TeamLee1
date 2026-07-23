using UnityEngine;

namespace Eric.ScriptableScripts
{
        public enum StageUpgradeType
        {
                Health,
                Attack,
                MeteoriteFragment,
                SatelliteAttackSpeed
        }

        [CreateAssetMenu(fileName = "New StageUpgradeSO", menuName = "Eric/StageUpgradeSO")]
        public class StageUpgradeSO : ScriptableObject
        {
                [field:Header("UI")]
                [field:SerializeField] public string UpgradeName{get;private set;}
                [field:SerializeField] public Texture Icon{get;private set;}

                [field:Header("Stat")]
                [field:SerializeField] public StageUpgradeType StageUpgradeType{get;private set;}
                [field:SerializeField] public float BaseStat{get;private set;} = 100f;
                [field:SerializeField] public bool IsIntStat{get;private set;} = true;
                [field:SerializeField] public int MaxLevel{get;private set;} = 10;
                [field:SerializeField] public float MultiplyPerLevel{get;private set;} = 1.1f;

                [field:Header("Cost")]
                [field:SerializeField] public int BaseNeedMF{get;private set;} = 10;
                [field:SerializeField] public float NeedMFMultiply{get;private set;} = 1.5f;
        }
}