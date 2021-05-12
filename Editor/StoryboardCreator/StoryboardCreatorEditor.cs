#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    [CustomEditor(typeof(StoryboardCreator))]
    public class StoryboardCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.Space(10);
            var storyboardCreator = (StoryboardCreator)target; 
            if (GUILayout.Button("Take Screenshots"))
            {
                storyboardCreator.TakeScreenshots();
            }
        }
    }
}
#endif