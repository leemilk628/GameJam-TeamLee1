using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    [SerializeField] private CinemachineCamera obj;
    private bool ok;
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (ok)
            {
                obj.Priority *= -1;
            }
            else
            {
                obj.Priority *= -1;
            }

            ok = !ok;
        }
    }
}
