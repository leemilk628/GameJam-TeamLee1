using DG.Tweening;
using UnityEngine;

namespace Eric.DropItem
{
    public class ItemGetEffect : MonoBehaviour
    {
        [Header("퍼지는 연출")]
        [SerializeField] private float minSpreadDistance = 0.6f;
        [SerializeField] private float maxSpreadDistance = 1.2f;
        [SerializeField] private float spreadDuration = 0.2f;

        [Header("가방으로 이동")]
        [SerializeField] private float jumpPower = 2f;
        [SerializeField] private float duration = 0.8f;

        private static RectTransform bagRectTransform;
        private Sequence effectSequence;

        private void OnEnable()
        {
            if (!FindBag()) return;
            PlayGetEffect();
        }

        private bool FindBag()
        {
            if (bagRectTransform != null) return true;
            GameObject bagObject = GameObject.FindGameObjectWithTag("Bag");
            bagRectTransform = bagObject.GetComponent<RectTransform>();
            return true;
        }

        private void PlayGetEffect()
        {
            Camera mainCamera = Camera.main;
            Vector3 bagWorldPosition = GetBagWorldPosition(mainCamera);
            Vector3 spreadPosition = GetRandomSpreadPosition();
            effectSequence?.Kill();
            effectSequence = DOTween.Sequence();
            effectSequence.Append(transform.DOMove(spreadPosition, spreadDuration).SetEase(Ease.OutQuad));
            effectSequence.Append(transform.DOJump(bagWorldPosition, jumpPower, 1, duration).SetEase(Ease.InQuad));
            effectSequence.Join(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
            effectSequence.OnComplete(() => { Destroy(gameObject); });
        }

        private Vector3 GetRandomSpreadPosition()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            float minDistance = Mathf.Min(minSpreadDistance, maxSpreadDistance);
            float maxDistance = Mathf.Max(minSpreadDistance, maxSpreadDistance);
            float distance = Random.Range(minDistance, maxDistance);
            return transform.position + direction * distance;
        }

        private Vector3 GetBagWorldPosition(Camera mainCamera)
        {
            Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(null,bagRectTransform.position);
            screenPosition.z = Vector3.Dot(transform.position - mainCamera.transform.position, mainCamera.transform.forward);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = transform.position.z;
            return worldPosition;
        }

        private void OnDisable()
        {
            effectSequence?.Kill();
            effectSequence = null;
        }
    }
}