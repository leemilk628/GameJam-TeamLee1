using System;
using UnityEngine;

public class RainbowModeToggle : MonoBehaviour
{
    public static bool IsRainbowEnabled { get; private set; }

    public static event Action<bool> OnRainbowModeChanged;

    [Header("게임 시작 시 레인보우 모드")]
    [SerializeField] private bool startEnabled = false;

    private static bool initialized;

    private void Awake()
    {
        if (initialized)
            return;

        initialized = true;
        SetRainbowMode(startEnabled);
    }

    /// <summary>
    /// 버튼의 OnClick에 연결하면 됨.
    /// </summary>
    public void ToggleRainbowMode()
    {
        SetRainbowMode(!IsRainbowEnabled);
    }

    public static void SetRainbowMode(bool enabled)
    {
        IsRainbowEnabled = enabled;
        OnRainbowModeChanged?.Invoke(enabled);
    }

    // Enter Play Mode Options에서 Domain Reload를 꺼도
    // 이전 실행의 static 값이 남지 않도록 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        IsRainbowEnabled = false;
        OnRainbowModeChanged = null;
        initialized = false;
    }
}