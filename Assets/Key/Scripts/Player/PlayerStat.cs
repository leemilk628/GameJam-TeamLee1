using UnityEngine;

namespace Key.Scripts.Player {
    public class PlayerStat : MonoBehaviour {
        [field: SerializeField] public int AttackPower { get; private set; } = 10;

        [field: SerializeField] public float AttackSpeed { get; private set; } = 2f;

        [field: SerializeField] public float KnockbackPower { get; private set; } = 5f;

        public void AddAttackPower(int amount) {
            AttackPower = Mathf.Max(0, AttackPower + amount);
        }

        public void AddAttackSpeed(float amount) {
            AttackSpeed = Mathf.Max(0.01f, AttackSpeed + amount);
        }

        public void AddKnockbackPower(float amount) {
            KnockbackPower = Mathf.Max(0f, KnockbackPower + amount);
        }

        public void MultiplyAttackPower(float multiplier) {
            AttackPower = Mathf.Max(
                0,
                Mathf.RoundToInt(AttackPower * multiplier)
            );
        }

        public void MultiplyAttackSpeed(float multiplier) {
            AttackSpeed = Mathf.Max(
                0.01f,
                AttackSpeed * multiplier
            );
        }
    }
}