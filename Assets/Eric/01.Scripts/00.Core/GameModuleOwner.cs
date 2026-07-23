using UnityEngine;

namespace Eric.ModuleSystem
{
        [DefaultExecutionOrder(-1000)]
        public class GameModuleOwner : ModuleOwner
        {
                public static GameModuleOwner Instance{get;private set;}

                protected override void Awake()
                {
                        if (Instance != null && Instance != this)
                        {
                                Destroy(gameObject);
                                return;
                        }

                        Instance = this;
                        DontDestroyOnLoad(gameObject);

                        base.Awake();
                }

                private void OnDestroy()
                {
                        if (Instance == this)
                                Instance = null;
                }
        }
}