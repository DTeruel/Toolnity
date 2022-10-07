#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity.StoryboardCreator
{
    [CustomEditor(typeof(StoryboardCreator))]
    public class CustomInspector : Editor
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