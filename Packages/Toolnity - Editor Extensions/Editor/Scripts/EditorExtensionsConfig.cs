using UnityEditor;
using UnityEngine;

namespace Toolnity.EditorExtensions
{
    [CreateAssetMenu(menuName = "Toolnity/Editor Extensions Config", fileName = "Editor Extensions")]
    public class EditorExtensionsConfig : ScriptableObject
    {
        [SerializeField]
        private bool saveOnPlay = true;
        
        public bool SaveOnPlay
        {
            get => saveOnPlay;
            set
            {
                saveOnPlay = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }
        
        [SerializeField]
        private bool loadLoadSceneOnPlay;
        public bool LoadSceneOnPlay
        {
            get => loadLoadSceneOnPlay;
            set
            {
                loadLoadSceneOnPlay = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        [SerializeField]
        private string masterScene;
        public string MasterScene 
        {
            get => masterScene;
            set
            {
                masterScene = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }
        
        public string PreviousScene { get; set; }
    }
}