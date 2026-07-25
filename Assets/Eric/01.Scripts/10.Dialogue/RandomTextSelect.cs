using System;
using Eric.ScriptableScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Eric.Dialogue
{
        public class RandomTextSelect : MonoBehaviour
        {
                [field:SerializeField]private DialogueTextSO DialogueTextSO{get;set;}
                private float _timer;
                [SerializeField] private float time;
                [field:SerializeField] private Dialogue Dialogue{get;set;}
                private void Update()
                {
                        _timer += Time.deltaTime;
                        if (_timer > time)
                        {
                                _timer = 0f;
                                string  text = DialogueTextSO.texts[UnityEngine.Random.Range(0, DialogueTextSO.texts.Length)];
                                Dialogue.StringToDialogue(text);
                        }
                }
        }
}