using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Eric.MainMenu
{
    public class SettingPanelToggle : MonoBehaviour
    {
        [SerializeField] private GameObject settingPanel;
        public void Toggle()
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Toggle();
            }
        }
    }
}
