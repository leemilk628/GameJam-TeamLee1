using Key.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EarthHealthUI : MonoBehaviour
{
    [SerializeField] private EarthHealth player;
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthPer;

    private float maxHealth;
    
    
    void Update()
    {
        maxHealth = playerStat.MaxHealth;
        healthBar.maxValue = playerStat.MaxHealth;
        
        float percent = player.Health / maxHealth * 100f;
        healthPer.text = percent.ToString("F0");
        
        healthBar.value = player.Health;
    }
}
