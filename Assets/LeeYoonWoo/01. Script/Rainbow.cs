using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class Rainbow : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float speed = 0.2f;

    [SerializeField, Range(0f, 1f)]
    private float saturation = 1f;

    [SerializeField, Range(0f, 1f)]
    private float brightness = 1f;

    [SerializeField, Range(0f, 1f)]
    private float hueOffset = 0f;

    private Graphic targetGraphic;
    private float originalAlpha;

    private void Awake()
    {
        targetGraphic = GetComponent<Graphic>();
        originalAlpha = targetGraphic.color.a;
    }

    private void Update()
    {
        float hue = Mathf.Repeat(
            Time.unscaledTime * speed + hueOffset,
            1f
        );

        Color rainbowColor = Color.HSVToRGB(
            hue,
            saturation,
            brightness
        );

        // 원래 투명도 유지
        rainbowColor.a = originalAlpha;

        targetGraphic.color = rainbowColor;
    }
}