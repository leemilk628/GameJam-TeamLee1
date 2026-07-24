using Key.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EarthHealthUI : MonoBehaviour
{
    [SerializeField] private EarthHealth player;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthPer;

    private float maxHealth;

    void Start()
    {
        maxHealth = player.Health;
        healthBar.maxValue = player.Health;
    }
    
    void Update()
    {
        float percent = player.Health / maxHealth * 100f;
        healthPer.text = percent.ToString("F0");
        
        healthBar.value = player.Health;
    }
}
