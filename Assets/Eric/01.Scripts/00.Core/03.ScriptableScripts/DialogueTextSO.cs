using UnityEngine;

namespace Eric.ScriptableScripts
{
        [CreateAssetMenu(fileName = "New DialogueTextSO", menuName = "Eric/DialogueTextSO")]
        public class DialogueTextSO : ScriptableObject
        {
                public string[] texts;
        }
}