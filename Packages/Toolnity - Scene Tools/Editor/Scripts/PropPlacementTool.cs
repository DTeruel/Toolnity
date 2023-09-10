#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolnity.SceneTools
{
    public class PropPlacementTool : EditorWindow
    {
        private static GUIStyle titleToggle;
        private static Transform[] transformsToModify;
        private static Vector2 selectObjectScrollPosition;
        
        private static bool positionModify = true;
        private static float positionRaycastInitialY = 10f;
        private static float positionRaycastLength = 100f;
        private static float positionOffsetEndY;
        private static bool positionOffsetRelativeToScale = true;
        private static bool positionCheckCollidersLayer;
        private static string positionCollidersLayer = "Default";

        private static bool rotationModify = true;
        private static readonly bool[] RotationAxis = {true, true, true}; 
        private static readonly float[] RotationRange = {0f, 360f};
        
        private static bool scaleModify = true;
        private static readonly bool[] ScaleAxis = {true, true, true}; 
        private static readonly float[] ScaleRange = {0.5f, 2.0f}; 
        private static bool scaleUniform = true;

        [MenuItem("Tools/Toolnity/Prop Placement Tool", priority = 3000)]
        public static void ShowWindow()
        {
            var propPlacementToolWindow = GetWindow<PropPlacementTool>("Prop Placement Tool");
            propPlacementToolWindow.titleContent = new GUIContent("Prop Placement Tool", EditorGUIUtility.IconContent("d_TransformTool").image);
            propPlacementToolWindow.Show();
        }

        private void OnEnable()
        {
            UpdateSelectedObjects();
        }

        private void OnGUI()
        {
            CheckStyles();
            
            DrawPositionOptions();
            DrawRotationOptions();
            DrawScaleOptions();
            
            DrawSelectedObjects();
            DrawExecuteButton();
        }

        private static void CheckStyles()
        {
            if (titleToggle != null)
            {
                return;
            }
            
            titleToggle = new GUIStyle(GUI.skin.toggle)
            {
                fontStyle = FontStyle.Bold
            };
        }

        private static void DrawPositionOptions()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            positionModify = GUILayout.Toggle(positionModify, " POSITION", titleToggle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (!positionModify)
            {
                return;
            }
            
            GUILayout.Space(5f);
            
            // Ray Initial Y Pos
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Ray Initial Y Pos: ");
            positionRaycastInitialY = EditorGUILayout.FloatField(positionRaycastInitialY, GUILayout.Width(50f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
                
            // Ray Length
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Ray Length: ");
            positionRaycastLength = EditorGUILayout.FloatField(positionRaycastLength, GUILayout.Width(50f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
                
            // Offset Y End Position
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            GUILayout.Label("Offset Y End Position: ");
            positionOffsetEndY = EditorGUILayout.FloatField(positionOffsetEndY, GUILayout.Width(50f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
                
            // Offset Relative to Scale
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            positionOffsetRelativeToScale = EditorGUILayout.Toggle("Offset Relative to Scale: ", positionOffsetRelativeToScale);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
                
            // Filter Ground by Layer
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            positionCheckCollidersLayer = EditorGUILayout.Toggle("Check Colliders Layer: ", positionCheckCollidersLayer);
            if (positionCheckCollidersLayer)
            {
                GUILayout.Space(5f);
                positionCollidersLayer = EditorGUILayout.TextField(positionCollidersLayer, GUILayout.Width(100f));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void DrawRotationOptions()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            rotationModify = GUILayout.Toggle(rotationModify, " ROTATION", titleToggle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (!rotationModify)
            {
                return;
            }
            
            GUILayout.Space(5f);
            
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

        private static void DrawScaleOptions()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            scaleModify = GUILayout.Toggle(scaleModify, " SCALE", titleToggle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (!scaleModify)
            {
                return;
            }

            GUILayout.Space(5f);
            
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

        private static void DrawSelectedObjects()
        {            
            var objectToReplaceCount = transformsToModify?.Length ?? 0;

            GUILayout.Space(20f);
            GUILayout.Label("OBJECTS SELECTED: " + objectToReplaceCount, EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            selectObjectScrollPosition = EditorGUILayout.BeginScrollView(selectObjectScrollPosition);
            if (transformsToModify != null)
            {
                foreach(var go in transformsToModify)
                {
                    EditorGUILayout.ObjectField(go, typeof(GameObject), true);
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
        }

        private static void DrawExecuteButton()
        {
            GUILayout.Space(35f);
            
            DrawSelectObjectMessage();
            EditorGUI.BeginDisabledGroup(!AnyObjectSelected() || !AnyOptionSelected());
            if (GUILayout.Button("Execute"))
            {
                ExecuteTransforms();
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

        private static void ExecuteTransforms()
        {
            for (var i = 0; i < transformsToModify.Length; i++)
            {
                RandomizeRotation(i);
                RandomizeScale(i);
                SetPosition(i); // Important do that after the scale modification
            }
        }
        private static void RandomizeRotation(int i)
        {
            if (!rotationModify || (!RotationAxis[0] && !RotationAxis[1] && !RotationAxis[2]))
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
            Undo.RegisterCompleteObjectUndo(transformsToModify[i], "Saving game object state");
            transformsToModify[i].rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }

        private static void RandomizeScale(int i)
        {
            if (!scaleModify || (!ScaleAxis[0] && !ScaleAxis[1] && !ScaleAxis[2]))
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

        private static void SetPosition(int i)
        {
            if (!positionModify)
            {
                return;
            }

            var rayInitialPos = transformsToModify[i].localPosition;
            rayInitialPos.y = positionRaycastInitialY;
            var results = new RaycastHit[10];
            int numResults;
            if (positionCheckCollidersLayer)
            {
                var layerMaskInt = LayerMask.GetMask(positionCollidersLayer);
                numResults = Physics.RaycastNonAlloc(rayInitialPos, Vector3.down, results, positionRaycastLength, layerMaskInt);
            }
            else
            {
                numResults = Physics.RaycastNonAlloc(rayInitialPos, Vector3.down, results, positionRaycastLength);
            }
            Debug.Log("numResults " + numResults);
            if (numResults == 0)
            {
                return;
            }
            if (numResults == 1)
            {
                if (results[0].transform.gameObject == transformsToModify[i].gameObject)
                {
                    return;
                }
            }
            
            
            var firstResult = true; 
            var endPos = rayInitialPos;
            for (var j = 0; j < numResults; j++)
            {
                if (results[j].transform.gameObject == transformsToModify[i].gameObject)
                {
                    continue;
                }
                
                Debug.Log("Colliding with " + results[j].transform.name);
                
                if (firstResult)
                {
                    endPos.y = results[j].point.y;
                    
                    firstResult = false;
                    continue;
                }
                
                if (endPos.y < results[j].point.y)
                {
                    endPos.y = results[j].point.y;
                }
            }

            if (positionOffsetRelativeToScale)
            {
                endPos.y += positionOffsetEndY * transformsToModify[i].localScale.y;
            }
            else
            {
                endPos.y += positionOffsetEndY;
            }
            Undo.RegisterCompleteObjectUndo(transformsToModify[i], "Saving game object state");
            transformsToModify[i].localPosition = endPos;
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

        private static bool AnyOptionSelected()
        {
            return positionModify || rotationModify || scaleModify;
        }

        private static bool AnyObjectSelected()
        {
            return transformsToModify != null && transformsToModify.Length > 0;
        }
    }
}
#endif