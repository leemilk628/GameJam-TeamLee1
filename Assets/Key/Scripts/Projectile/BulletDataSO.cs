using UnityEngine;

namespace Key.Scripts.BulletSc {
    [CreateAssetMenu(fileName = "BulletData", menuName = "Key/SO/BulletData", order = 0)]
    public class BulletDataSO : ScriptableObject {
        [field: Header("Visual")]
        [field: SerializeField]
        public Sprite Sprite { get; private set; }

        [field: SerializeField]
        public float RotationOffset { get; private set; } = -90f;

        [field: Header("Stats")]
        [field: SerializeField, Min(0)]
        public int Damage { get; private set; } = 10;

        [field: SerializeField, Min(0.01f)]
        public float MoveSpeed { get; private set; } = 10f;

        [field: SerializeField, Min(0.01f)]
        public float LifeTime { get; private set; } = 3f;

        [field: SerializeField, Min(0f)]
        public float KnockbackPower { get; private set; } = 3f;
    }
}