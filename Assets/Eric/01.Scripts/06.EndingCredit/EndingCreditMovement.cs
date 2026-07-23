using UnityEngine;
using UnityEngine.InputSystem;

namespace Eric.EndingCredit
{
    public class EndingCreditMovement : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Rigidbody2D rb;

        private void FixedUpdate()
        {
            rb.linearVelocityY = Keyboard.current.spaceKey.isPressed ? speed * 5 : speed;
        }
    }
}
