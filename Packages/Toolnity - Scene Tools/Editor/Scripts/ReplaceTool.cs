#if UNITY_EDITOR
// Original Code from https://www.patrykgalach.com/2019/08/26/replace-tool-for-level-designers/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Toolnity.SceneTools
{
    public class ReplaceData : ScriptableObject
    {
        public GameObject replaceObject;
        public GameObject[] objectsToReplace;
    }

    public class ReplaceTool : EditorWindow
    {
        private Vector2 selectObjectScrollPosition;
        private ReplaceData data;
        private SerializedObject serializedData;
        private SerializedProperty replaceObjectField;

        [MenuItem("Tools/Toolnity/Replace Tool", priority = 3000)]
        public static void ShowWindow()
        {
            var replaceToolWindow = GetWindow<ReplaceTool>("Replace Tool");
            replaceToolWindow.titleContent = new GUIContent("Replace Tool", EditorGUIUtility.IconContent("d_RotateTool On").image);
            replaceToolWindow.Show();
        }

        private void InitDataIfNeeded()
        {
            if (!data)
            {
                data = CreateInstance<ReplaceData>();
                serializedData = null;
            }
            if (serializedData == null)
            {
                serializedData = new SerializedObject(data);
                replaceObjectField = null;
            }
            if (replaceObjectField == null)
            {
                replaceObjectField = serializedData.FindProperty("replaceObject");
            }
        }
        
        private void OnGUI()
        {
            InitDataIfNeeded();
            
            DrawOriginalPrefab();
            DrawSelectedObjects();
            var replaceButtonEnabled = DrawHintMessages();
            DrawReplaceButton(replaceButtonEnabled);
            EditorGUILayout.Separator();
            
            serializedData.ApplyModifiedProperties();
        }

        private void DrawOriginalPrefab()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Original GameObject or Prefab", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(replaceObjectField);
        }

        private void DrawSelectedObjects()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Selected objects to replace", EditorStyles.boldLabel);
            
            GUI.enabled = false;
            
            var objectToReplaceCount = data.objectsToReplace?.Length ?? 0;
            EditorGUILayout.IntField("Object count", objectToReplaceCount);
            
            EditorGUI.indentLevel++;
            selectObjectScrollPosition = EditorGUILayout.BeginScrollView(selectObjectScrollPosition);
            if (data && data.objectsToReplace != null)
            {
                foreach(var go in data.objectsToReplace)
                {
                    EditorGUILayout.ObjectField(go, typeof(GameObject), true);
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUI.indentLevel--;
            
            GUI.enabled = true;
        }

        private bool DrawHintMessages()
        {
            EditorGUI.indentLevel++;
            
            var replaceButtonEnabled = true;
            if (!data.replaceObject)
            {
                replaceButtonEnabled = false;
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("*** Set a GameObject or Prefab to replace the selected objects ***", EditorStyles.centeredGreyMiniLabel);
            }
            
            var objectToReplaceCount = data.objectsToReplace?.Length ?? 0;
            if (objectToReplaceCount == 0)
            {
                if (replaceButtonEnabled)
                {
                    replaceButtonEnabled = false;
                    EditorGUILayout.Separator();
                }
                EditorGUILayout.LabelField("*** Select a GameObject/s in the Scene to replace it ***", EditorStyles.centeredGreyMiniLabel);
            }
            
            EditorGUI.indentLevel--;

            return replaceButtonEnabled;
        }

        private void DrawReplaceButton(bool replaceButtonEnabled)
        {
            EditorGUILayout.Separator();
            GUI.enabled = replaceButtonEnabled;
            if (GUILayout.Button("Replace"))
            {
                if (!replaceObjectField.objectReferenceValue)
                {
                    Debug.LogErrorFormat("[Replace Tool] {0}", "Missing prefab to replace with!");
                    return;
                }
                if (data.objectsToReplace.Length == 0)
                {
                    Debug.LogErrorFormat("[Replace Tool] {0}", "Missing objects to replace!");
                    return;
                }
                ReplaceSelectedObjects(data.objectsToReplace, data.replaceObject);
            }
            GUI.enabled = true;
        }

        private void OnSelectionChange()
        {
            InitDataIfNeeded();
            
            const SelectionMode objectFilter = SelectionMode.Unfiltered ^ ~(SelectionMode.Assets | SelectionMode.DeepAssets | SelectionMode.Deep);
            var selection = Selection.GetTransforms(objectFilter);
            data.objectsToReplace = selection.Select(s => s.gameObject).ToArray();
            if (serializedData.UpdateIfRequiredOrScript())
            {
                Repaint();
            }
        }

        private void OnInspectorUpdate()
        {
            if (serializedData != null && serializedData.UpdateIfRequiredOrScript())
            {
                Repaint();
            }
        }

        private static void ReplaceSelectedObjects(IReadOnlyList<GameObject> objectToReplace, GameObject replaceObject)
        {
            Debug.Log("[Replace Tool] Replace in progress...");
            
            var isObject = false;
            for (int i = 0; i < objectToReplace.Count; i++)
            {
                var go = objectToReplace[i];
                Undo.RegisterCompleteObjectUndo(go, "Saving game object state");
                var inst = (GameObject)PrefabUtility.InstantiatePrefab(replaceObject, go.transform.parent);
                if (!inst)
                {
                    isObject = true;
                    inst = Instantiate(replaceObject, go.transform.position, go.transform.rotation, go.transform.parent);
                }
                else
                {
                    inst.transform.position = go.transform.position;
                    inst.transform.rotation = go.transform.rotation;
                }
                inst.transform.localScale = go.transform.localScale;

                Undo.RegisterCreatedObjectUndo(inst, "Replacement creation.");
                foreach(Transform child in go.transform)
                {
                    Undo.SetTransformParent(child, inst.transform, "Parent Change");
                }
                Undo.DestroyObjectImmediate(go);
            }

            if (isObject)
            {
                Debug.LogFormat("[Replace Tool] {0} objects replaced on Scene with the object: {1}", objectToReplace.Count, replaceObject.name);
            }
            else
            {
                Debug.LogFormat("[Replace Tool] {0} objects replaced on Scene with the prefab: {1}", objectToReplace.Count, replaceObject.name);
            }
        }
    }
}
#endif