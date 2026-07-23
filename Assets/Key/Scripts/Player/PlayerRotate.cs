using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Key.Scripts.Player {
    public class PlayerRotate : MonoBehaviour {
        [Header("Mouse Rotation")]
        [SerializeField] private Camera mainCamera;

        [SerializeField] private float rotationOffset = -90f;

        private float _angle;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update() {
            RotateTowardMouse();
        }

        private void RotateTowardMouse()
        {
            if (Mouse.current == null || mainCamera == null)
                return;

            Vector2 mouseScreenPosition =
                Mouse.current.position.ReadValue();

            float distanceFromCamera = Mathf.Abs(
                transform.position.z - mainCamera.transform.position.z
            );

            Vector3 mouseWorldPosition =
                mainCamera.ScreenToWorldPoint(
                    new Vector3(
                        mouseScreenPosition.x,
                        mouseScreenPosition.y,
                        distanceFromCamera
                    )
                );

            Vector2 direction =
                (Vector2)mouseWorldPosition - (Vector2)transform.position;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            float mouseAngle =
                Mathf.Atan2(direction.y, direction.x)
                * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                mouseAngle + rotationOffset
            );
        }
    }
}