using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class RainbowUI : MonoBehaviour
{
    [Header("레인보우 설정")]

    [Tooltip("1초에 변화하는 색상 범위. 0.2면 약 5초에 한 바퀴")]
    [SerializeField, Min(0f)]
    private float rainbowSpeed = 0.2f;

    [Tooltip("UI마다 색상 시작점을 다르게 만들 때 사용")]
    [SerializeField, Range(0f, 1f)]
    private float hueOffset = 0f;

    [SerializeField, Range(0f, 1f)]
    private float saturation = 1f;

    [SerializeField, Range(0f, 1f)]
    private float brightness = 1f;

    private Graphic targetGraphic;
    private Color originalColor;
    private bool rainbowEnabled;

    private void Awake()
    {
        targetGraphic = GetComponent<Graphic>();
        originalColor = targetGraphic.color;
    }

    private void OnEnable()
    {
        RainbowModeToggle.OnRainbowModeChanged += SetRainbowMode;
        SetRainbowMode(RainbowModeToggle.IsRainbowEnabled);
    }

    private void OnDisable()
    {
        RainbowModeToggle.OnRainbowModeChanged -= SetRainbowMode;
    }

    private void Update()
    {
        if (!rainbowEnabled)
            return;

        float hue = Mathf.Repeat(
            Time.unscaledTime * rainbowSpeed + hueOffset,
            1f
        );

        Color rainbowColor = Color.HSVToRGB(
            hue,
            saturation,
            brightness
        );

        // 기존 UI 투명도 유지
        rainbowColor.a = originalColor.a;

        targetGraphic.color = rainbowColor;
    }

    private void SetRainbowMode(bool enabled)
    {
        rainbowEnabled = enabled;

        if (!rainbowEnabled && targetGraphic != null)
        {
            targetGraphic.color = originalColor;
        }
    }

    /// <summary>
    /// 다른 스크립트가 기본 색상을 변경했을 때 호출.
    /// </summary>
    public void SaveCurrentColorAsOriginal()
    {
        if (targetGraphic != null)
        {
            originalColor = targetGraphic.color;
        }
    }
}