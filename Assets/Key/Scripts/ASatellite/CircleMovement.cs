using UnityEngine;

namespace Key.Scripts.Player
{
    public class CircleMovement : MonoBehaviour
    {
        [Header("Circle Movement")]
        [SerializeField] private Transform center;
        [SerializeField] private float radius = 3f;
        [SerializeField] private float angularSpeed = 90f;
        [SerializeField] private float startAngle = 0f;
        
        private float _angle;

        private void Start()
        {
            _angle = startAngle;
        }

        private void Update()
        {
            MoveAroundCenter();
        }

        private void MoveAroundCenter()
        {
            if (center == null)
                return;

            _angle += angularSpeed * Time.deltaTime;

            float radian = _angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radian) * radius;
            float y = Mathf.Sin(radian) * radius;

            transform.position =
                center.position + new Vector3(x, y, 0f);
        }
    }
}