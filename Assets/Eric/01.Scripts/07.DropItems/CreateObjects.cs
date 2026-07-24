using System;
using Eric.ModuleSystem;
using UnityEngine;

namespace Eric.DropItems
{
        public class CreateObjects : MonoBehaviour, IModule
        {
                public SelectImageSO selectImageSO;

                public event Action HandleCreateObject;
                
                
                private void CreateObject()
                {
                        for(int i = 0; i < selectImageSO.Count; i++)
                        {
                                Instantiate(selectImageSO.SelectedObject);
                        }
                }

                public void COEvent()
                {
                        HandleCreateObject?.Invoke();
                }

                public void Init(ModuleOwner owner)
                {
                        HandleCreateObject += CreateObject;
                }

                public void AfterInit()
                {
                }

                private void OnDisable()
                {
                        HandleCreateObject -= CreateObject;
                }
        }
}