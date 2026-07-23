using System;
using UnityEngine;

namespace Key.Scripts.Player {
    public class PlayerAttack : MonoBehaviour {
        public float AttackPower { get; private set; }
        public float AttackSpeed { get; private set; }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float rayDistance = 100f;

        private void Awake() {
            if (mainCamera == null) {
                mainCamera = Camera.main;
            }
        }

        private void OnAttack() {
            Vector2 mouseWorldPosition =
                mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 origin = transform.position;
            Vector2 direction = (mouseWorldPosition - origin).normalized;

            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                direction,
                rayDistance,
                targetLayer
            );

            Debug.DrawRay(
                origin,
                direction * rayDistance,
                Color.red
            );

            if (hit.collider != null) {
                Debug.Log("충돌한 오브젝트: " + hit.collider.name);

                if (Input.GetMouseButtonDown(0)) {
                    Debug.Log(hit.collider.name + " 클릭됨");
                }
            }
        }
    }
}
