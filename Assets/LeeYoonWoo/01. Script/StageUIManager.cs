using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class StageUIManager : MonoBehaviour
{
    public static StageUIManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private RectTransform stageTextRect;

    [Header("이동 시간")]
    [Tooltip("왼쪽에서 중앙 근처까지 들어오는 시간")]
    [SerializeField] private float enterDuration = 0.35f;

    [Tooltip("중앙을 천천히 지나가는 시간")]
    [SerializeField] private float centerDuration = 0.8f;

    [Tooltip("오른쪽으로 빠져나가는 시간")]
    [SerializeField] private float exitDuration = 0.3f;

    [Header("이동 범위")]
    [Tooltip("중앙 감속 구간의 너비")]
    [SerializeField] private float centerZoneWidth = 400f;

    [Tooltip("화면 바깥으로 얼마나 더 이동할지")]
    [SerializeField] private float outsidePadding = 100f;

    [Header("설정")]
    [Tooltip("게임이 일시정지되어도 연출을 실행할지")]
    [SerializeField] private bool useUnscaledTime = true;

    private RectTransform parentRect;
    private Coroutine animationCoroutine;
    private float originalY;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (stageTextRect == null && stageText != null)
        {
            stageTextRect = stageText.rectTransform;
        }

        if (stageTextRect == null)
        {
            Debug.LogError("StageUIManager에 Stage Text를 연결해야 합니다.");
            return;
        }

        parentRect = stageTextRect.parent as RectTransform;
        originalY = stageTextRect.anchoredPosition.y;

        stageTextRect.gameObject.SetActive(false);
    }

    /// <summary>
    /// 스테이지 텍스트 연출을 실행한다.
    /// </summary>
    public void NextStageText(string text)
    {
        if (stageText == null || stageTextRect == null || parentRect == null)
            return;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        stageText.text = text;
        stageTextRect.gameObject.SetActive(true);

        animationCoroutine = StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        // 텍스트 크기와 캔버스 크기를 최신 상태로 갱신
        Canvas.ForceUpdateCanvases();
        stageText.ForceMeshUpdate();

        float parentHalfWidth = parentRect.rect.width * 0.5f;
        float textHalfWidth = stageTextRect.rect.width * 0.5f;

        float startX =
            -parentHalfWidth
            - textHalfWidth
            - outsidePadding;

        float centerStartX = -centerZoneWidth * 0.5f;
        float centerEndX = centerZoneWidth * 0.5f;

        float endX =
            parentHalfWidth
            + textHalfWidth
            + outsidePadding;

        SetPosition(startX);

        // 빠르게 들어오다가 중앙 근처에서 감속
        yield return MoveX(
            startX,
            centerStartX,
            enterDuration,
            EaseOutCubic
        );

        // 중앙을 천천히 통과
        yield return MoveX(
            centerStartX,
            centerEndX,
            centerDuration,
            Linear
        );

        // 다시 가속하면서 오른쪽으로 퇴장
        yield return MoveX(
            centerEndX,
            endX,
            exitDuration,
            EaseInCubic
        );

        stageTextRect.gameObject.SetActive(false);
        animationCoroutine = null;
    }

    private IEnumerator MoveX(
        float fromX,
        float toX,
        float duration,
        Func<float, float> easing
    )
    {
        if (duration <= 0f)
        {
            SetPosition(toX);
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            float deltaTime = useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            timer += deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float easedT = easing(t);

            float currentX = Mathf.LerpUnclamped(
                fromX,
                toX,
                easedT
            );

            SetPosition(currentX);

            yield return null;
        }

        SetPosition(toX);
    }

    private void SetPosition(float x)
    {
        stageTextRect.anchoredPosition = new Vector2(
            x,
            originalY
        );
    }

    private static float Linear(float t)
    {
        return t;
    }

    // 빠르게 시작한 뒤 감속
    private static float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    // 천천히 시작한 뒤 가속
    private static float EaseInCubic(float t)
    {
        return t * t * t;
    }
}