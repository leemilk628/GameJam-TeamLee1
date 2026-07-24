using System.Collections;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    [Header("창")]
    [SerializeField] private RectTransform window1;
    [SerializeField] private RectTransform window2;

    [Header("애니메이션")]
    [SerializeField] private float closeDuration = 0.2f;
    [SerializeField] private float openDuration = 0.25f;

    // 창이 열렸을 때의 원래 값
    private Vector3 window1OpenPosition;
    private Vector3 window2OpenPosition;

    private Vector3 window1OpenScale;
    private Vector3 window2OpenScale;

    // 창마다 자신의 왼쪽 아래 위치
    private Vector3 window1ClosedPosition;
    private Vector3 window2ClosedPosition;

    private int currentWindow = 1;
    private bool isAnimating;

    private void Awake()
    {
        // Inspector에 설정해둔 현재 위치와 스케일 저장
        window1OpenPosition = window1.localPosition;
        window2OpenPosition = window2.localPosition;

        window1OpenScale = window1.localScale;
        window2OpenScale = window2.localScale;

        // 현재 창 모양을 기준으로 왼쪽 아래 지점 계산
        window1ClosedPosition = GetBottomLeftPosition(window1);
        window2ClosedPosition = GetBottomLeftPosition(window2);

        // 처음에는 1번 창만 열기
        window1.gameObject.SetActive(true);
        window1.localPosition = window1OpenPosition;
        window1.localScale = window1OpenScale;

        window2.localPosition = window2ClosedPosition;
        window2.localScale = Vector3.zero;
        window2.gameObject.SetActive(false);
    }

    // 1번 버튼에 연결
    public void OpenWindow1()
    {
        // 이미 1번 창이거나 애니메이션 중이면 무시
        if (currentWindow == 1 || isAnimating)
            return;

        StartCoroutine(ChangeWindow(
            closeWindow: window2,
            closePosition: window2ClosedPosition,
            openWindow: window1,
            openPosition: window1OpenPosition,
            openScale: window1OpenScale,
            nextWindowNumber: 1
        ));
    }

    // 2번 버튼에 연결
    public void OpenWindow2()
    {
        // 이미 2번 창이거나 애니메이션 중이면 무시
        if (currentWindow == 2 || isAnimating)
            return;

        StartCoroutine(ChangeWindow(
            closeWindow: window1,
            closePosition: window1ClosedPosition,
            openWindow: window2,
            openPosition: window2OpenPosition,
            openScale: window2OpenScale,
            nextWindowNumber: 2
        ));
    }

    private IEnumerator ChangeWindow(
        RectTransform closeWindow,
        Vector3 closePosition,
        RectTransform openWindow,
        Vector3 openPosition,
        Vector3 openScale,
        int nextWindowNumber)
    {
        isAnimating = true;

        // 현재 창이 자기 왼쪽 아래로 작아지면서 닫힘
        yield return AnimateTransform(
            closeWindow,
            closeWindow.localPosition,
            closePosition,
            closeWindow.localScale,
            Vector3.zero,
            closeDuration
        );

        closeWindow.gameObject.SetActive(false);

        // 다음 창을 자기 왼쪽 아래에서 시작
        openWindow.gameObject.SetActive(true);
        openWindow.localPosition = GetClosedPosition(openWindow);
        openWindow.localScale = Vector3.zero;

        // 원래 위치와 원래 크기로 커지면서 열림
        yield return AnimateTransform(
            openWindow,
            openWindow.localPosition,
            openPosition,
            Vector3.zero,
            openScale,
            openDuration
        );

        openWindow.localPosition = openPosition;
        openWindow.localScale = openScale;

        currentWindow = nextWindowNumber;
        isAnimating = false;
    }

    private IEnumerator AnimateTransform(
        RectTransform target,
        Vector3 startPosition,
        Vector3 endPosition,
        Vector3 startScale,
        Vector3 endScale,
        float duration)
    {
        if (duration <= 0f)
        {
            target.localPosition = endPosition;
            target.localScale = endScale;
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            // 부드럽게 가속하고 감속
            t = t * t * (3f - 2f * t);

            target.localPosition =
                Vector3.LerpUnclamped(startPosition, endPosition, t);

            target.localScale =
                Vector3.LerpUnclamped(startScale, endScale, t);

            yield return null;
        }

        target.localPosition = endPosition;
        target.localScale = endScale;
    }

    private Vector3 GetClosedPosition(RectTransform window)
    {
        if (window == window1)
            return window1ClosedPosition;

        return window2ClosedPosition;
    }

    private Vector3 GetBottomLeftPosition(RectTransform window)
    {
        Vector3[] corners = new Vector3[4];
        window.GetWorldCorners(corners);

        // 왼쪽 아래와 왼쪽 위의 중간 = 왼쪽 중앙
        Vector3 leftWorldPosition = (corners[0] + corners[1]) * 0.5f;

        RectTransform parent = window.parent as RectTransform;

        if (parent == null)
            return window.localPosition;

        Vector3 leftLocalPosition =
            parent.InverseTransformPoint(leftWorldPosition);

        leftLocalPosition.z = window.localPosition.z;

        return leftLocalPosition;
    }
}