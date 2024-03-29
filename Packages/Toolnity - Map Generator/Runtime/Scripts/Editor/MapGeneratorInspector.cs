#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity.MapGenerator
{
    [CustomEditor(typeof(MapGenerator))]
    internal class MapGeneratorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Clear and Generate"))
            {
                var mapGen = (MapGenerator)target;
                mapGen.ClearAndGenerate();
            }
            if (GUILayout.Button("Clear"))
            {
                var mapGen = (MapGenerator)target;
                mapGen.Clear();
            }
            if (GUILayout.Button("Generate"))
            {
                var mapGen = (MapGenerator)target;
                mapGen.Generate();
            }
        }
    }
}
#endif