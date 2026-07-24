using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Eric.GameOver
{
    public class LightFading : MonoBehaviour
    {
        private Light2D _light2D;
        private bool _canFade;
        [SerializeField] private TextMeshProUGUI text;

        private void OnEnable()
        {
            _light2D = GetComponent<Light2D>();
        }

        public void State()
        {
            _canFade = true;
        }

        private void Update()
        {
            switch (_canFade)
            {
                case true:
                    FadeOut();
                    break;
                case false when _light2D.intensity >= 0:
                    Fadein();
                    break;
            }
        }

        private void FadeOut()
        {
            _light2D.intensity += Time.deltaTime * 4;
            if(_light2D.intensity >= 30) _canFade = false;
            text.DOFade(1, 6f);
        }

        private void Fadein()
        {
            _light2D.intensity -= Time.deltaTime * 4;
            text.DOFade(0, 6f);
        }
    }
}
