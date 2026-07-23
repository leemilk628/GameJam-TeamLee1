using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using UnityEngine;

namespace Eric.Upgrade
{
        public class DetectClick : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private SkillTreeUpgradeModule _skillTreeUpgradeModule;

                [field:SerializeField] public SkillTreeSO SkillTree{get;private set;}

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                }

                public void AfterInit()
                {
                        _skillTreeUpgradeModule = Owner.GetModule<SkillTreeUpgradeModule>();
                }

                public void Upgrade()
                {
                        if (Owner == null || SkillTree == null || _skillTreeUpgradeModule == null)
                                return;

                        _skillTreeUpgradeModule.TryUpgrade(SkillTree);
                }
        }
}