using JetBrains.Annotations;
using UnityEngine;

namespace LeeYoonWoo._01._Script
{
    [CreateAssetMenu(fileName = "StageData", menuName = "LeeYoonWoo/StageData", order = 0)]
    public class StageData : ScriptableObject
    {
        [Header("Stage")] public int Stage;

        [Header("Stage Data")]
        public float StageTime;
        
        public float[] BreakTime = new float[2];
        public float[] BossSpawnTime =  new float[2];
        private bool[] SpawnedBoss =  new bool[2];

        public GameObject[] Enemies;
        public GameObject[] Boss1 =  new GameObject[1];
        public GameObject[] Boss2 =   new GameObject[1];

        public float SpawnTerm;


        public void Init()
        {
            SpawnedBoss[0] = false;
            SpawnedBoss[1] = false;
        }
        
        
        public GameObject[] StageState(float time)
        {
            if ((BreakTime[0] <= time && BreakTime[0] + 10 >= time) || BreakTime[1] <= time && BreakTime[1] + 10 >= time)
            {
                return null;
            }
            
            if (time >= BossSpawnTime[0] && SpawnedBoss[0] == false)
            {
                SpawnedBoss[0] = true;
                return Boss1;
            }
            if (time >= BossSpawnTime[1] && SpawnedBoss[1] == false)
            {
                SpawnedBoss[1] = true;
                return Boss2;
            }

            return Enemies;
        }
    }
}