using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Eric.ModuleSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Eric.SkillTreeUpgrade
{
        public class SizeChanger:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler, IModule
        {
                private ModuleOwner Owner{get;set;}
                private Vector3 MyScale{get;set;}
                [field: SerializeField] private float MaxSize { get; set; } = 1.15f;
                [field: SerializeField] private float Speed { get; set; } = 0.7f;
                public void OnPointerEnter(PointerEventData eventData)
                {
                        transform.DOScale(Vector3.one * MaxSize, Speed);
                }

                public void OnPointerExit(PointerEventData eventData)
                {
                        transform.DOScale(MyScale, Speed);
                }

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        MyScale = transform.localScale;
                }

                public void AfterInit()
                {
                }
        }
}