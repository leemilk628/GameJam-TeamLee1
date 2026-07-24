using UnityEngine;
using UnityEngine.InputSystem;

namespace Eric.DropItems
{
        public class TempCreate : MonoBehaviour
        {
                private void Update()
                {
                        if (Keyboard.current.spaceKey.wasPressedThisFrame)
                        {
                                DropReward();
                        }
                }

                private void DropReward()
                {
                        if (CreateObjects.Instance == null) return;

                        int meteoriteAmount = 20;
                        int goldAmount = 50;

                        CreateObjects.Instance.CreateEvent(meteoriteAmount, goldAmount, transform.position);
                }
        }
}