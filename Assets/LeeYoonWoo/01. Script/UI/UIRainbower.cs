using UnityEngine;
using UnityEngine.UI;

public class UIRainbower : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private float speed = 0.3f;

    private void Update()
    {
        float hue = Mathf.Repeat(Time.time * speed, 1f);
        img.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}
