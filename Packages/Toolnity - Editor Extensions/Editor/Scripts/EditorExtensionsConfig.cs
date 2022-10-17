using System;
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
                EditorUtility.SetDirty(this);
            }
        }
        
        [SerializeField]
        private bool sceneOnPlay = true;
        public bool SceneOnPlay
        {
            get => sceneOnPlay;
            set
            {
                sceneOnPlay = value;
                EditorUtility.SetDirty(this);
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
                EditorUtility.SetDirty(this);
            }
        }
        
        public string PreviousScene { get; set; }
    }
}