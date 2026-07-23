using UnityEngine;

namespace Key.Scripts.Player {
    public class PlayerMovement : MonoBehaviour {
        [SerializeField] private Transform center; // 원의 중심
        [SerializeField] private float radius = 3f; // 반지름
        [SerializeField] private float angularSpeed = 90f; // 초당 회전 각도
        [SerializeField] private float startAngle = 0f;

        private float angle;

        private void Start() {
            angle = startAngle;
        }

        private void Update() {
            angle += angularSpeed * Time.deltaTime;

            float radian = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radian) * radius;
            float y = Mathf.Sin(radian) * radius;

            transform.position = center.position + new Vector3(x, y, 0f);
        }
    }
}
