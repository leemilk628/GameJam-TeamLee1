using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Eric.ScriptableScripts
{
        public enum SkillTreeType
        {
                PlayerHealth,
                PlayerBarrier,
                PlayerAttack,
                PlayerAttackSpeed,
                SatelliteAttack,
                SatelliteAttackSpeed,
                Satellite,
                GetMeteoriteFragment,
                GetGold,
                MaxSatelliteCount,
                StartingGold,
                BarrierRecoverySpeed
        }

        public enum SatelliteType
        {
                None,
                AutocannonSatellite,
                MissileSatellite,
                LaserSatellite,
                BasicSatellite
        }

        [CreateAssetMenu(fileName = "New SkillTreeSO", menuName = "Eric/SkillTreeSO")]
        public class SkillTreeSO : ScriptableObject
        {
                [Header("Save")]
                [SerializeField] private string nodeId;

                [field:Header("Skill Data")]
                [field:SerializeField] public SkillTreeType SkillTreeType{get;private set;}
                [field:SerializeField] public int IncreaseValue{get;private set;} = 1;
                [field:SerializeField] public SatelliteType SatelliteType{get;private set;}
                [field:SerializeField] public SkillTreeSO BeforeNode{get;private set;}
                [field:SerializeField] public bool IsUpgrade{get;private set;}
                [field:SerializeField] public string Description{get;private set;}

                [field: Header("Cost")]
                [field: FormerlySerializedAs("<NeedGold>k__BackingField")]
                [field: SerializeField] public int NeedMeteoriteFragment { get; private set; } = 50;

                public string NodeId => nodeId;

                public void Upgrade()
                {
                        IsUpgrade = true;
                }

                public void SetUpgradeState(bool isUpgrade)
                {
                        IsUpgrade = isUpgrade;
                }

#if UNITY_EDITOR
                private void OnValidate()
                {
                        if (!string.IsNullOrWhiteSpace(nodeId))
                                return;

                        nodeId = Guid.NewGuid().ToString();
                        UnityEditor.EditorUtility.SetDirty(this);
                }
#endif
        }
}