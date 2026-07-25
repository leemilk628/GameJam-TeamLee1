using System.Collections;
using Eric.Scenes;
using LeeYoonWoo._01._Script;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    
    private int curStage = 0;
    
    public StageData[] stage = new StageData[4];
    
    int T = 1;
    
    void Start()
    {
        Debug.Log("처음 스테이지 시작됨.");
        StageUIManager.Instance.NextStageText($"WAVE {curStage+1}");
        stageManager.stageData = stage[curStage];
        stageManager.Init();
        stageManager.isStage = true;
    }

    void Update()
    {
        if (stageManager.timer >= stageManager.stageData.StageTime && stageManager.isStage)
        {
            stageManager.isStage = false;
            stageManager.Pbar.maxValue = T;
            stageManager.Pbar.value = T;
            StartCoroutine(WaitNextStage(5f));
        }
    }

    void MoveToNextStage()
    {
        curStage++;
        if (curStage+1 == 5)
        {
            SceneChanger.Instance.GoToEnding();
        }
        Debug.Log($"{curStage+1}스테이지 시작됨.");
        StageUIManager.Instance.NextStageText($"WAVE {curStage+1}");
        stageManager.stageData = stage[curStage];
        stageManager.Init();
        stageManager.isStage = true;
    }
    
    private IEnumerator WaitNextStage(float time)
    {
        yield return new WaitForSeconds(time);
        //다음 스테이지 쉬는시간 넘어갈때 효과 넣기
        yield return Count60();
        MoveToNextStage();
    }

    private IEnumerator Count60()
    {
        Debug.Log("다음 스테이지까지의 기다림 시작됨.");
        for (int i = T; i >=1; i--)
        {
            stageManager.Pbar.value = i;
            yield return new WaitForSeconds(1f);
        }
    }
}
