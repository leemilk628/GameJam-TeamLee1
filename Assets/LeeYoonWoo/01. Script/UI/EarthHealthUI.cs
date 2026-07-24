using Key.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EarthHealthUI : MonoBehaviour
{
    [SerializeField] private EarthHealth player;
    [SerializeField] private PlayerStat playerStat;
    
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider barrierBar;
    
    [SerializeField] private TextMeshProUGUI healthPer;

    private float maxHealth;
    private float maxBarrier;
    
    
    void Update()
    {
        maxHealth = playerStat.MaxHealth;
        healthBar.maxValue = playerStat.MaxHealth;

        maxBarrier = player.MaxBarrier;
        barrierBar.maxValue = player.MaxBarrier;
        
        float percent = (player.Health + player.Barrier) / maxHealth * 100f;

        if (percent > 100)
        {
            healthPer.color = Color.dodgerBlue;
        }
        else
        {
            healthPer.color = Color.white;
        }
        
        healthPer.text = percent.ToString("F0");
        
        healthBar.value = player.Health;
        barrierBar.value = player.Barrier;
    }
}
