using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Eric.EndingCredit
{
        public class Fades : MonoBehaviour, IFadeable
        {
                [SerializeField] private float fadeSpeed;
                [SerializeField] private Image fadeImage;

                public void FadeIn()
                {
                        fadeImage.DOFade(0, fadeSpeed);
                }

                public void FadeOut()
                {
                        fadeImage.DOFade(1, fadeSpeed);
                }
        }
}