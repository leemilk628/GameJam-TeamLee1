using UnityEngine;
using UnityEngine.Serialization;

namespace Eric.ScriptableScripts
{
        public enum StageUpgradeType
        {
                Health,
                HealthRecovery,
                Attack,
                AttackSpeed,
                Barrier,
                MeteoriteFragment,
                SatelliteAttack,
                SatelliteAttackSpeed,
                MaxSatelliteCount
        }

        [CreateAssetMenu(fileName = "New StageUpgradeSO", menuName = "Eric/StageUpgradeSO")]
        public class StageUpgradeSO : ScriptableObject
        {
                [field:Header("UI")]
                [field:SerializeField] public string UpgradeName{get;private set;}
                [field:SerializeField] public Sprite Icon{get;private set;}

                [field:Header("Stat")]
                [field:SerializeField] public StageUpgradeType StageUpgradeType{get;private set;}
                [field:SerializeField] public int BaseStat{get;private set;} = 1;
                [field:SerializeField] public int AddValuePerLevel{get;private set;} = 1;
                [field:SerializeField] public int MaxLevel{get;private set;} = 10;

                [field:Header("Cost")]
                [field:FormerlySerializedAs("<BaseNeedMF>k__BackingField")]
                [field:SerializeField] public int BaseNeedGold{get;private set;} = 10;

                [field:FormerlySerializedAs("<NeedMFMultiply>k__BackingField")]
                [field:SerializeField] public float NeedGoldMultiply{get;private set;} = 1.5f;
        }
}