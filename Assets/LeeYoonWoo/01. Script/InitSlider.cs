using UnityEngine;
using UnityEngine.UI;

public class InitSlider : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void Init(float MaxValue, float curValue)
    {
        slider.maxValue = MaxValue;
        slider.value = curValue;
    }
}
