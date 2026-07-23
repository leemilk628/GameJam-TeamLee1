using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eric.ScriptableScripts
{

        [CreateAssetMenu(fileName = "New SkillTreeImageSO", menuName = "Eric/SKillTreeImageSO", order = 0)]
        public class SkillTreeImageSO : ScriptableObject
        {
                [field: SerializeField] public List<Sprite> Nodes { get; private set; } = new();
                public Sprite GetNode(int num)
                {
                        return Nodes[num];
                }
        }
}