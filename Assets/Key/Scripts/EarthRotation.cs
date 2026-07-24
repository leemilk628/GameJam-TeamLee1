using UnityEngine;

namespace Key.Scripts {
    public class EarthRotation : MonoBehaviour {
        [SerializeField] private float rotationSpeed;
        
        private void Update() {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }
}