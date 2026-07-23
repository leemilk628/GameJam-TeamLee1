using System;
using UnityEngine;

namespace Eric.ScriptableScripts
{
        [Serializable]
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

        [Serializable]
        public enum SatelliteType{
                Moon,
        }
        [CreateAssetMenu(fileName = "New SkillTreeSO", menuName = "Eric/SkillTreeSO")]
        public class SkillTreeSO : ScriptableObject
        {
                [field:SerializeField]public SkillTreeType SkillTreeType { get; private set; }
                [field:SerializeField]public float Multiply { get; private set; }
                [field:SerializeField]public SatelliteType SatelliteType { get; private set; }
                [field:SerializeField]public bool IsUpgrade { get; set; }
                [field:SerializeField]public bool CanUpgrade { get; set; }
        }
}