using Unity.Cinemachine;
using UnityEngine;

public class CMDInteracter : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cineCam;

    public void Click()
    {
        cineCam.Priority *= -1;
    }
}
