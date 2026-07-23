using UnityEngine;

namespace Eric.EndingCredit
{
        public class DetectFade : MonoBehaviour
        {
                private void OnTriggerEnter2D(Collider2D other)
                {
                        if (other.TryGetComponent(out IFadeable fadeable))
                        {
                                fadeable.FadeOut();
                        }
                }
        }
}