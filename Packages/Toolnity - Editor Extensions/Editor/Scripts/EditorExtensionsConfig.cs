using UnityEngine;

namespace Toolnity.EditorExtensions
{
    [CreateAssetMenu(menuName = "Toolnity/Editor Extensions Config", fileName = "Editor Extensions")]
    public class EditorExtensionsConfig : ScriptableObject
    {
        public bool autoSaveOnPlay = true;
        public bool loadSceneOnPlay;
        public string masterScene;
        public string previousScene;
    }
}