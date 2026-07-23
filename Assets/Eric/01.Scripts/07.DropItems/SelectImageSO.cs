using UnityEngine;

namespace Eric.DropItems
{
        [CreateAssetMenu(fileName = "New SelectImageSO", menuName = "Eric/DropItems/SelectImageSO")]
        public class SelectImageSO : ScriptableObject
        {
                [field:SerializeField] public GameObject  SelectedObject { get; private set; }
                [field:SerializeField] public int Count { get; private set; }
        }
}