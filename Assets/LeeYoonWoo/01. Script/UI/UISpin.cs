using UnityEngine;

public class UISpin : MonoBehaviour
{
    [SerializeField] private float rotSpeed;
    [SerializeField] private RectTransform rt;

    void Update()
    {
        rt.Rotate(0f, 0f, rotSpeed * Time.deltaTime);
    }
}
