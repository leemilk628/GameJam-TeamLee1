using System;
using Eric.ScriptableScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Eric.Dialogue
{
        public class LineTextSelect : MonoBehaviour
        {
                [field:SerializeField]private DialogueTextSO DialogueTextSO{get;set;}
                private float _timer;
                [SerializeField] private float time;
                [field:SerializeField] private Dialogue Dialogue{get;set;}
                [SerializeField] private int line = 0;
                private void Update()
                {
                        _timer += Time.deltaTime;
                        if (_timer > time)
                        {
                                _timer = 0f;
                                string  text = DialogueTextSO.texts[line];
                                Dialogue.StringToDialogue(text);
                                line = line >= DialogueTextSO.texts.Length-1 ? 0 : ++line;
                        }
                }
        }
}