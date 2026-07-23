using UnityEngine;

namespace Key.Scripts.ASatellite {
    public class ASatellite1 : AbstractASatellite {
        protected void OnSatelliteInitialized() {
            Debug.Log($"{name} 초기화 완료");
        }

    }
}