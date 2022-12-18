using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolnity.ProjectInfo
{
    [CreateAssetMenu(menuName = "Toolnity/Project Info Config", fileName = "Project Info")]
    public class ProjectInfoConfig : ScriptableObject
    {
        [SerializeField]
        private string projectName;
        public string ProjectName 
        {
            get => projectName;
            set
            {
                projectName = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
                ProjectInfo.RegenerateMenu();
#endif
            }
        }

        [Serializable]
        public struct LinkInfo
        {
            public string Name;
            public string URL;
        }
        
        [SerializeField]
        private List<LinkInfo> links;
        public List<LinkInfo> Links
        {
            get => links;
            set
            {
                links = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
                ProjectInfo.RegenerateMenu();
#endif
            }
        }
    }
}