using Eric.ModuleSystem;
using UnityEngine;

namespace Eric.StageUpgrade
{
        [DefaultExecutionOrder(-500)]
        public class StageModuleOwner : ModuleOwner
        {
                public static StageModuleOwner Instance{get;private set;}

                protected override void Awake()
                {
                        Instance = this;
                        base.Awake();
                }

                private void OnDestroy()
                {
                        if (Instance == this)
                                Instance = null;
                }
        }
}