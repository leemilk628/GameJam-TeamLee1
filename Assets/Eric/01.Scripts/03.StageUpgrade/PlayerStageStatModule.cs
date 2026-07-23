using System;
using Eric.ModuleSystem;
using Eric.StageUpgrade;
using UnityEngine;

namespace Eric.Player
{
        public class PlayerStageStatModule : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private StageUpgradeModule _stageUpgradeModule;
                private float _barrierRecoveryTimer;

                [field:SerializeField] public int BaseMaxHealth{get;private set;} = 100;
                [field:SerializeField] public int BaseAttack{get;private set;} = 10;
                [field:SerializeField] public int BaseAttackSpeed{get;private set;} = 1;
                [field:SerializeField] public int BaseBarrier{get;private set;} = 100;
                [field:SerializeField] public int BaseBarrierRecoverySpeed{get;private set;} = 1;

                public int MaxHealth{get;private set;}
                public int CurrentHealth{get;private set;}
                public int Attack{get;private set;}
                public int AttackSpeed{get;private set;}
                public int MaxBarrier{get;private set;}
                public int CurrentBarrier{get;private set;}
                public int BarrierRecoverySpeed{get;private set;}

                public event Action OnStatsChanged;
                public event Action<int, int> OnHealthChanged;
                public event Action<int, int> OnBarrierChanged;

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
                        _stageUpgradeModule.OnPlayerHealthRecoveryRequested += Heal;

                        InitializeStats();
                }

                private void Update()
                {
                        RecoverBarrier();
                }

                private void OnDestroy()
                {
                        if (_stageUpgradeModule == null)
                                return;

                        _stageUpgradeModule.OnStageUpgradeDataChanged -= RecalculateStats;
                        _stageUpgradeModule.OnPlayerHealthRecoveryRequested -= Heal;
                }

                public void TakeDamage(int damage)
                {
                        if (damage <= 0 || CurrentHealth <= 0)
                                return;

                        int barrierDamage = Mathf.Min(CurrentBarrier, damage);

                        CurrentBarrier -= barrierDamage;
                        CurrentHealth = Mathf.Max(0, CurrentHealth - (damage - barrierDamage));
                        _barrierRecoveryTimer = 0f;

                        NotifyChanged();
                }

                public void Heal(int amount)
                {
                        if (amount <= 0 || CurrentHealth <= 0)
                                return;

                        int previousHealth = CurrentHealth;
                        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);

                        if (previousHealth != CurrentHealth)
                                NotifyChanged();
                }

                public void RestoreBarrier(int amount)
                {
                        if (amount <= 0 || CurrentHealth <= 0)
                                return;

                        int previousBarrier = CurrentBarrier;
                        CurrentBarrier = Mathf.Min(MaxBarrier, CurrentBarrier + amount);

                        if (previousBarrier != CurrentBarrier)
                                NotifyChanged();
                }

                public void SetCurrentHealth(int value)
                {
                        CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);
                        NotifyChanged();
                }

                public void SetCurrentBarrier(int value)
                {
                        CurrentBarrier = Mathf.Clamp(value, 0, MaxBarrier);
                        _barrierRecoveryTimer = 0f;
                        NotifyChanged();
                }

                public void ResetCurrentStats()
                {
                        CurrentHealth = MaxHealth;
                        CurrentBarrier = MaxBarrier;
                        _barrierRecoveryTimer = 0f;
                        NotifyChanged();
                }

                private void InitializeStats()
                {
                        MaxHealth = _stageUpgradeModule.GetPlayerMaxHealth(BaseMaxHealth);
                        Attack = _stageUpgradeModule.GetPlayerAttack(BaseAttack);
                        AttackSpeed = _stageUpgradeModule.GetPlayerAttackSpeed(BaseAttackSpeed);
                        MaxBarrier = _stageUpgradeModule.GetPlayerBarrier(BaseBarrier);
                        BarrierRecoverySpeed = _stageUpgradeModule.GetPlayerBarrierRecoverySpeed(BaseBarrierRecoverySpeed);

                        CurrentHealth = MaxHealth;
                        CurrentBarrier = MaxBarrier;
                        _barrierRecoveryTimer = 0f;

                        NotifyChanged();
                }

                private void InitializeBaseStats()
                {
                        MaxHealth = BaseMaxHealth;
                        CurrentHealth = BaseMaxHealth;
                        Attack = BaseAttack;
                        AttackSpeed = BaseAttackSpeed;
                        MaxBarrier = BaseBarrier;
                        CurrentBarrier = BaseBarrier;
                        BarrierRecoverySpeed = BaseBarrierRecoverySpeed;
                        _barrierRecoveryTimer = 0f;

                        NotifyChanged();
                }

                private void RecalculateStats()
                {
                        int previousHealth = CurrentHealth;
                        int previousBarrier = CurrentBarrier;

                        MaxHealth = _stageUpgradeModule.GetPlayerMaxHealth(BaseMaxHealth);
                        Attack = _stageUpgradeModule.GetPlayerAttack(BaseAttack);
                        AttackSpeed = _stageUpgradeModule.GetPlayerAttackSpeed(BaseAttackSpeed);
                        MaxBarrier = _stageUpgradeModule.GetPlayerBarrier(BaseBarrier);
                        BarrierRecoverySpeed = _stageUpgradeModule.GetPlayerBarrierRecoverySpeed(BaseBarrierRecoverySpeed);

                        CurrentHealth = Mathf.Clamp(previousHealth, 0, MaxHealth);
                        CurrentBarrier = Mathf.Clamp(previousBarrier, 0, MaxBarrier);

                        NotifyChanged();
                }

                private void RecoverBarrier()
                {
                        if (CurrentHealth <= 0 || CurrentBarrier >= MaxBarrier || BarrierRecoverySpeed <= 0)
                        {
                                _barrierRecoveryTimer = 0f;
                                return;
                        }

                        _barrierRecoveryTimer += Time.deltaTime;

                        if (_barrierRecoveryTimer < 1f)
                                return;

                        int recoveryCount = Mathf.FloorToInt(_barrierRecoveryTimer);
                        _barrierRecoveryTimer -= recoveryCount;

                        RestoreBarrier(BarrierRecoverySpeed * recoveryCount);
                }

                private void NotifyChanged()
                {
                        OnStatsChanged?.Invoke();
                        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
                        OnBarrierChanged?.Invoke(CurrentBarrier, MaxBarrier);
                }
        }
}