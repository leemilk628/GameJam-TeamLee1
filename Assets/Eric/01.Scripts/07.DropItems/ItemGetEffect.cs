using System;
using DG.Tweening;
using Eric.DropItems;
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

        private static Transform bagTransform;
        private Sequence effectSequence;
        private Vector3 initialScale;
        private bool initialized;
        private bool collected;
        private Action<ItemGetEffect> onCollected;
        private Action<ItemGetEffect> onDisabled;

        public DropItemType ItemType { get; private set; }
        public int RewardAmount { get; private set; }

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        private void OnEnable()
        {
            transform.localScale = initialScale;
        }

        public void Initialize(
            DropItemType type,
            int amount,
            float delay,
            Action<ItemGetEffect> collectedCallback,
            Action<ItemGetEffect> disabledCallback)
        {
            effectSequence?.Kill();

            ItemType = type;
            RewardAmount = Mathf.Max(0, amount);
            onCollected = collectedCallback;
            onDisabled = disabledCallback;
            initialized = true;
            collected = false;

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            transform.localScale = initialScale;

            if (!PlayEffect(Mathf.Max(0f, delay)))
                Complete();
        }

        private bool PlayEffect(float delay)
        {
            if (!FindBag()) return false;

            Vector3 spreadPosition = GetSpreadPosition();
            Vector3 bagPosition = bagTransform.position;
            bagPosition.z = transform.position.z;

            effectSequence = DOTween.Sequence();
            effectSequence.SetDelay(delay);
            effectSequence.Append(transform.DOMove(spreadPosition, spreadDuration).SetEase(Ease.OutQuad));
            effectSequence.Append(transform.DOJump(bagPosition, jumpPower, 1, duration).SetEase(Ease.InQuad));
            effectSequence.Join(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
            effectSequence.OnComplete(Complete);

            return true;
        }

        private static bool FindBag()
        {
            if (bagTransform != null) return true;

            GameObject bag = GameObject.FindGameObjectWithTag("Bag");

            if (bag == null) return false;

            bagTransform = bag.transform;
            return true;
        }

        private Vector3 GetSpreadPosition()
        {
            float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 direction = new(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            float min = Mathf.Min(minSpreadDistance, maxSpreadDistance);
            float max = Mathf.Max(minSpreadDistance, maxSpreadDistance);

            return transform.position + direction * UnityEngine.Random.Range(min, max);
        }

        private void Complete()
        {
            if (!initialized || collected) return;

            collected = true;
            onCollected?.Invoke(this);
        }

        private void OnDisable()
        {
            effectSequence?.Kill();
            effectSequence = null;

            if (!initialized) return;

            Action<ItemGetEffect> callback = onDisabled;

            initialized = false;
            collected = false;
            onCollected = null;
            onDisabled = null;

            callback?.Invoke(this);
        }
    }
}