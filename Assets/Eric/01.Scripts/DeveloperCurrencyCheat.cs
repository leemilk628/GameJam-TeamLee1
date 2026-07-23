using Eric.Currency;
using Eric.ModuleSystem;
using Eric.StageUpgrade;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Eric.Developer
{
        public class DeveloperCurrencyCheat : MonoBehaviour
        {
                [field:SerializeField] public int AddAmount{get;private set;} = 1000;

                private GoldModule _goldModule;
                private MeteoriteFragmentModule _meteoriteFragmentModule;

                private void Update()
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        Keyboard keyboard = Keyboard.current;

                        if (keyboard == null)
                                return;

                        bool isAllPressed =
                                keyboard.numpad1Key.isPressed &&
                                keyboard.numpad2Key.isPressed &&
                                keyboard.numpad3Key.isPressed;

                        bool isPressedThisFrame =
                                keyboard.numpad1Key.wasPressedThisFrame ||
                                keyboard.numpad2Key.wasPressedThisFrame ||
                                keyboard.numpad3Key.wasPressedThisFrame;

                        if (!isAllPressed || !isPressedThisFrame)
                                return;

                        AddCurrency();
#endif
                }

                private void AddCurrency()
                {
                        FindModules();

                        _goldModule.AddGold(AddAmount);
                        _meteoriteFragmentModule.AddMeteoriteFragment(AddAmount);
                }

                private void FindModules()
                {
                        if (_goldModule == null && GameModuleOwner.Instance != null)
                        {
                                _goldModule =
                                        GameModuleOwner.Instance.GetModule<GoldModule>();
                        }

                        if (_meteoriteFragmentModule == null && StageModuleOwner.Instance != null)
                        {
                                _meteoriteFragmentModule =
                                        StageModuleOwner.Instance.GetModule<MeteoriteFragmentModule>();
                        }
                }
        }
}