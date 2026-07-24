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
        [SerializeField] private float fadeSpeed;
        [SerializeField] private float intensitySize;
        [SerializeField] private float duration;
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
            _light2D.intensity += Time.deltaTime * fadeSpeed;
            if(_light2D.intensity >= intensitySize) _canFade = false;
            text.DOFade(1, duration);
        }

        private void Fadein()
        {
            _light2D.intensity -= Time.deltaTime * fadeSpeed;
            text.DOFade(0, duration);
        }
    }
}
