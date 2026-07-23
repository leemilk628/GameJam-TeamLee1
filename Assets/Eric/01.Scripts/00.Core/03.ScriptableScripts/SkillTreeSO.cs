using System;
using UnityEngine;

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
                GetGold
        }

        public enum SatelliteType
        {
                None,
                Moon
        }

        [CreateAssetMenu(fileName = "New SkillTreeSO", menuName = "Eric/SkillTreeSO")]
        public class SkillTreeSO : ScriptableObject
        {
                [Header("Save")]
                [SerializeField] private string nodeId;

                [field:Header("Skill Data")]
                [field:SerializeField] public SkillTreeType SkillTreeType{get;private set;}
                [field:SerializeField] public float Multiply{get;private set;} = 1f;
                [field:SerializeField] public SatelliteType SatelliteType{get;private set;}
                [field:SerializeField] public SkillTreeSO BeforeNode{get;private set;}
                [field:SerializeField] public bool IsUpgrade{get;private set;}

                [field:Header("Cost")]
                [field:SerializeField] public int NeedGold{get;private set;}

                public string NodeId => nodeId;

                public void Upgrade() => IsUpgrade = true;

                public void SetUpgradeState(bool isUpgrade) => IsUpgrade = isUpgrade;

                
#if UNITY_EDITOR
                private void OnValidate()
                {
                        if (string.IsNullOrWhiteSpace(nodeId))
                        {
                                nodeId = Guid.NewGuid().ToString();
                                UnityEditor.EditorUtility.SetDirty(this);
                        }
                }
#endif
        }
}