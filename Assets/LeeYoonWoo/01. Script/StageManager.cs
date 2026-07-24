using JetBrains.Annotations;
using LeeYoonWoo._01._Script;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float timer;
    
    [SerializeField] private InitSlider MainProgressBar;
    [SerializeField] private InitSlider SafeMark_Left1;
    [SerializeField] private InitSlider SafeMark_Right1;
    [SerializeField] private InitSlider SafeMark_Left2;
    [SerializeField] private InitSlider SafeMark_Right2;
    [SerializeField] private InitSlider WarningMark1;
    [SerializeField] private InitSlider WarningMark2;
    
    private bool isSpawn = false;

    [CanBeNull] private GameObject[] entity;

    private Slider Pbar;

    void Awake()
    {
        stageData.Init();
        Pbar = MainProgressBar.GetComponent<Slider>();
    }

    void Start()
    {
        MainProgressBar.Init(stageData.StageTime, 0);
        SafeMark_Left1.Init(stageData.StageTime, stageData.BreakTime[0]);
        SafeMark_Right1.Init(stageData.StageTime, stageData.BreakTime[0]+10);
        SafeMark_Left2.Init(stageData.StageTime, stageData.BreakTime[1]);
        SafeMark_Right2.Init(stageData.StageTime, stageData.BreakTime[1]+10);
        WarningMark1.Init(stageData.StageTime, stageData.BossSpawnTime[0]);
        WarningMark2.Init(stageData.StageTime, stageData.BossSpawnTime[1]);
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        Pbar.value = timer;
        entity = stageData.StageState(timer);

        if (entity != null && entity.Length == 1)
        {
            spawner.Spawn(entity);
        }
        
        if (entity != null && (int)timer % stageData.SpawnTerm == 0 && entity.Length != 1)
        {
            if (isSpawn) return;
            isSpawn = true;
            spawner.Spawn(entity);
        }
        else
        {
            isSpawn = false;
        }
    }
}
