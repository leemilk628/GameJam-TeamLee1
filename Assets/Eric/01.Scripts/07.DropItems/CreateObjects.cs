using UnityEngine;

namespace Eric.DropItems
{
        public class CreateObjects : MonoBehaviour
        {
                public SelectImageSO selectImageSO;
                public void CreateObject()
                {
                        for(int i = 0; i < selectImageSO.Count; i++)
                        {
                                Instantiate(selectImageSO.SelectedObject);
                        }
                }
        }
}