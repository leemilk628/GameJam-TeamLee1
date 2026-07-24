using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Eric.Dialogue
{
    public class Dialogue : MonoBehaviour
    {
        public static Dialogue Instance { get; private set; }
        
        [SerializeField] private float typingSpeed = 0.1f;
        [SerializeField] private float afterTypingWaitTime = 1f;
        
        private StringBuilder text = new();
        
        [field:SerializeField] private TextMeshProUGUI DialogueText { get; set; }

        private Coroutine co;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void StringToDialogue(string texts)
        {
            char[] chars = texts.ToCharArray();
            DialogueText.text = "";
            text.Clear();
            if(co != null)StopCoroutine(co);
            co = StartCoroutine(StringText(chars));
        }

        private IEnumerator StringText(char[] chars)
        {
            foreach (char c in chars)
            {
                text.Append(c);
                DialogueText.text = text.ToString();
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(afterTypingWaitTime);
            text.Clear();
            DialogueText.text = "";
        }
    }
}
