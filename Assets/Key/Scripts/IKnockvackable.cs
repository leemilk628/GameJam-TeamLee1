namespace Key.Scripts {
    using UnityEngine;

    public interface IKnockbackable
    {
        void Knockback(Vector2 direction, float power);
    }
}