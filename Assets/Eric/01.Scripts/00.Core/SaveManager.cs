using System;
using Eric.ModuleSystem;
using Eric.StageUpgrade;
using UnityEngine;

namespace Eric.Save
{
        public class SaveManager : MonoBehaviour, IModule
        {
                private ModuleOwner Owner { get; set; }
                [SerializeField] private SaveSkillTree _saveSkillTree;
                [SerializeField] private StageUpgradeModule _stageUpgradeModule;
                private event Action HandleOnSave;
                private event Action HandleOnLoad;
                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                }

                public void AfterInit()
                {
                        DontDestroyOnLoad(gameObject);
                }
                public void RaiseSave(Action T)
                {
                        HandleOnSave += T;
                }
                public void RaiseLoad(Action T)
                {
                        HandleOnLoad += T;
                }

                public void InvokeSave()
                {
                        HandleOnSave?.Invoke();
                }

                public void InvokeLoad()
                {
                        HandleOnLoad?.Invoke();
                }

                public void ResetAll()
                {
                        _saveSkillTree.ResetSaveData();
                }

                public void ResetStageData()
                {
                        _stageUpgradeModule.ResetStageData();
                }

                private void OnDestroy()
                {
                        HandleOnSave = null;
                        HandleOnLoad = null;
                }
        }
}