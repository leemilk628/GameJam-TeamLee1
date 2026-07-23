using UnityEngine;

namespace Key.Scripts.ASatellite {
    [CreateAssetMenu(fileName = "ASatelliteData", menuName = "Key/SO/ASatelliteData", order = 0)]
    public class ASatelliteSO : ScriptableObject {
        
        [Header("Attack")]
        public int attackPower;
        public float attackSpeed;
        public GameObject bullet;
        public float speed;

        [Header("Enforce")]
        public int maxEnforce = 3;

        public float[] damageIncreaseAmount = new float [3];
        public float[] attackSpeedIncreaseAmount = new float [3];
        
        [Header("Buy")]
        public int price;

        public int[] upgradePrice = new int [3];

        [Header("Info")] 
        public string name;
        public string desc;
    }
}