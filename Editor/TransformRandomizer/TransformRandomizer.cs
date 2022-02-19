#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
    public class TransformRandomizer : EditorWindow
    {
        private static TransformRandomizer transformRandomizerWindow;
        private static Transform[] transformsToModify;
        private static readonly bool[] ScaleAxis = {true, true, true}; 
        private static readonly float[] ScaleRange = {0.5f, 2.0f}; 
        private static bool scaleUniform = true; 
        private static readonly bool[] RotationAxis = {true, true, true}; 
        private static readonly float[] RotationRange = {0f, 360f};
        private static Vector2 selectObjectScrollPosition;

        [MenuItem("Tools/Toolnity/Transform Randomizer", priority = 1500)]
        public static void ShowWindow()
        {
            transformRandomizerWindow = GetWindow<TransformRandomizer>("Replace Tool");
            transformRandomizerWindow.titleContent = new GUIContent("Transform Randomizer", EditorGUIUtility.IconContent("d_TransformTool").image);
            transformRandomizerWindow.Show();
        }

        private void OnEnable()
        {
            UpdateSelectedObjects();
        }

        private void OnGUI()
        {
            DrawScaleOptions();
            DrawRotationOptions();
            DrawSelectedObjects();
            DrawRandomizeButton();
        }

        private static void DrawScaleOptions()
        {
            GUILayout.Space(10f);
            GUILayout.Label("SCALE:", EditorStyles.boldLabel);
            
            // Axis
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Axis: ");
            GUILayout.Space(13f);
            ScaleAxis[0] = GUILayout.Toggle(ScaleAxis[0] ,"X");
            GUILayout.Space(13f);
            ScaleAxis[1] = GUILayout.Toggle(ScaleAxis[1] ,"Y");
            GUILayout.Space(13f);
            ScaleAxis[2] = GUILayout.Toggle(ScaleAxis[2] ,"Z");
            GUILayout.Space(25f);
            scaleUniform = GUILayout.Toggle(scaleUniform ,"Uniform");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
                
            // Min-Max
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Range: ");
            ScaleRange[0] = EditorGUILayout.FloatField(ScaleRange[0], GUILayout.Width(50f));
            GUILayout.Label("-");
            ScaleRange[1] = EditorGUILayout.FloatField(ScaleRange[1], GUILayout.Width(50f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void DrawRotationOptions()
        {
            GUILayout.Space(20f);
            GUILayout.Label("ROTATION:", EditorStyles.boldLabel);
            
            // Axis
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Axis: ");
            GUILayout.Space(13f);
            RotationAxis[0] = GUILayout.Toggle(RotationAxis[0] ,"X");
            GUILayout.Space(13f);
            RotationAxis[1] = GUILayout.Toggle(RotationAxis[1] ,"Y");
            GUILayout.Space(13f);
            RotationAxis[2] = GUILayout.Toggle(RotationAxis[2] ,"Z");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
                
            // Min-Max
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Range: ");
            RotationRange[0] = EditorGUILayout.FloatField(RotationRange[0], GUILayout.Width(50f));
            if (RotationRange[0] < 0)
            {
                RotationRange[0] = 0;
            }
            GUILayout.Label("-");
            RotationRange[1] = EditorGUILayout.FloatField(RotationRange[1], GUILayout.Width(50f));
            if (RotationRange[1] > 360)
            {
                RotationRange[1] = 360;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void DrawSelectedObjects()
        {            
            var objectToReplaceCount = transformsToModify?.Length ?? 0;

            GUILayout.Space(20f);
            GUILayout.Label("OBJECTS SELECTED: " + objectToReplaceCount, EditorStyles.boldLabel);
            
            GUI.enabled = false;
            EditorGUI.indentLevel++;
            selectObjectScrollPosition = EditorGUILayout.BeginScrollView(selectObjectScrollPosition);
            if (transformsToModify != null)
            {
                foreach(var go in transformsToModify)
                {
                    EditorGUILayout.ObjectField(go, typeof(GameObject), true);
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
            GUI.enabled = true;
        }

        private static void DrawRandomizeButton()
        {
            GUILayout.Space(35f);
            
            DrawSelectObjectMessage();
            EditorGUI.BeginDisabledGroup(!AnyObjectSelected());
            if (GUILayout.Button("Randomize"))
            {
                RandomizeTransforms();
            }
            EditorGUILayout.Separator();
            EditorGUI.EndDisabledGroup();
        }

        private static void DrawSelectObjectMessage()
        {
            if (AnyObjectSelected())
            {
                return;
            }
            EditorGUILayout.LabelField("*** Select at least one object ***", EditorStyles.centeredGreyMiniLabel);
        }

        private static void RandomizeTransforms()
        {
            for (var i = 0; i < transformsToModify.Length; i++)
            {
                RandomizeScale(i);
                RandomizeRotation(i);
            }
        }

        private static void RandomizeScale(int i)
        {
            if (!ScaleAxis[0] && !ScaleAxis[1] && !ScaleAxis[2])
            {
                return;
            }

            var scale = transformsToModify[i].localScale;
            var uniformScale = Random.Range(ScaleRange[0], ScaleRange[1]);
            if (ScaleAxis[0])
            {
                if (scaleUniform)
                {
                    scale.x = uniformScale;
                }
                else
                {
                    scale.x = Random.Range(ScaleRange[0], ScaleRange[1]);
                }
            }
            if (ScaleAxis[1])
            {
                if (scaleUniform)
                {
                    scale.y = uniformScale;
                }
                else
                {
                    scale.y = Random.Range(ScaleRange[0], ScaleRange[1]);
                }
            }
            if (ScaleAxis[2])
            {
                if (scaleUniform)
                {
                    scale.z = uniformScale;
                }
                else
                {
                    scale.z = Random.Range(ScaleRange[0], ScaleRange[1]);
                }
            }
            Undo.RegisterCompleteObjectUndo(transformsToModify[i], "Saving game object state");
            transformsToModify[i].localScale = scale;
        }

        private static void RandomizeRotation(int i)
        {
            if (!RotationAxis[0] && !RotationAxis[1] && !RotationAxis[2])
            {
                return;
            }

            var rotation = Vector3.one;
            if (RotationAxis[0])
            {
                rotation.x = Random.Range(RotationRange[0], RotationRange[1]);
            }
            if (RotationAxis[1])
            {
                rotation.y = Random.Range(RotationRange[0], RotationRange[1]);
            }
            if (RotationAxis[2])
            {
                rotation.y = Random.Range(RotationRange[0], RotationRange[1]);
            }
            transformsToModify[i].rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            Undo.RegisterCompleteObjectUndo(transformsToModify[i], "Saving game object state");
        }

        private void OnSelectionChange()
        {
            UpdateSelectedObjects();
            Repaint();
        }

        private static void UpdateSelectedObjects()
        {
            const SelectionMode objectFilter = SelectionMode.Unfiltered ^ ~(SelectionMode.Assets | SelectionMode.DeepAssets | SelectionMode.Deep);
            var selection = Selection.GetTransforms(objectFilter);
            transformsToModify = selection.Select(s => s).ToArray();
        }

        private static bool AnyObjectSelected()
        {
            return transformsToModify != null && transformsToModify.Length > 0;
        }
    }
}
#endif