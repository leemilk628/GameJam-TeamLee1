using System;
using Eric.ModuleSystem;
using Eric.StageUpgrade;
using UnityEngine;

namespace Eric.Satellite
{
        public class SatelliteStageStatModule : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private StageUpgradeModule _stageUpgradeModule;

                [field:SerializeField] public int BaseAttack{get;private set;} = 10;
                [field:SerializeField] public int BaseAttackSpeed{get;private set;} = 1;
                [field:SerializeField] public int BaseMaxSatelliteCount{get;private set;} = 1;

                public int Attack{get;private set;}
                public int AttackSpeed{get;private set;}
                public int MaxSatelliteCount{get;private set;}

                public event Action OnStatsChanged;

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                }

                public void AfterInit()
                {
                        _stageUpgradeModule = Owner.GetModule<StageUpgradeModule>();

                        if (_stageUpgradeModule == null)
                        {
                                InitializeBaseStats();
                                return;
                        }

                        _stageUpgradeModule.OnStageUpgradeDataChanged += RecalculateStats;
                        RecalculateStats();
                }

                private void OnDestroy()
                {
                        if (_stageUpgradeModule != null)
                                _stageUpgradeModule.OnStageUpgradeDataChanged -= RecalculateStats;
                }

                public bool CanSpawnSatellite(int currentSatelliteCount)
                {
                        return currentSatelliteCount < MaxSatelliteCount;
                }

                private void InitializeBaseStats()
                {
                        Attack = BaseAttack;
                        AttackSpeed = BaseAttackSpeed;
                        MaxSatelliteCount = BaseMaxSatelliteCount;
                        OnStatsChanged?.Invoke();
                }

                private void RecalculateStats()
                {
                        Attack = _stageUpgradeModule.GetSatelliteAttack(BaseAttack);
                        AttackSpeed = _stageUpgradeModule.GetSatelliteAttackSpeed(BaseAttackSpeed);
                        MaxSatelliteCount = _stageUpgradeModule.GetMaxSatelliteCount(BaseMaxSatelliteCount);

                        OnStatsChanged?.Invoke();
                }
        }
}