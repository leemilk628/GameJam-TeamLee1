using System;
using System.Collections.Generic;

namespace Eric.Save
{
        [Serializable]
        public class SkillTreeSaveEntry
        {
                public string nodeId;
                public bool isUpgrade;
        }

        [Serializable]
        public class SkillTreeSaveData
        {
                public List<SkillTreeSaveEntry> skillTreeSaveEntries = new();
        }
}