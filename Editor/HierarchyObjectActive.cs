#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Toolnity
{
	[InitializeOnLoad]
	public static class HierarchyObjectActive
	{
		public const string HIERARCHY_OBJECT_SETTINGS_ENABLED = "Toolnity/Hierarchy Object/Enabled";
		
		private const float ButtonSize = 15;
		private const float ButtonOffset = 2 + ButtonSize * 2;

		static HierarchyObjectActive()
		{
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
		}

		private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
		{
			var enabledOption = EditorPrefs.GetBool(Application.dataPath + HIERARCHY_OBJECT_SETTINGS_ENABLED, true);
			if (!enabledOption)
			{
				return;
			}

			var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (gameObject == null)
			{
				return;
			}

			// Avoid Prefab Root
			if (selectionRect.y == 0 && gameObject.transform.childCount > 0)
			{
				return;
			}
			
			var r = new Rect(selectionRect) {x = ButtonOffset, width = ButtonSize};
			var value = GUI.Toggle(r, gameObject.activeSelf, "");

			if (gameObject.activeSelf != value)
			{
				gameObject.SetActive(value);

				CheckSelectedObjectsActive(gameObject, value);

				MakeSceneDirty();
				MakePrefabDirty();
			}
		}

		private static void CheckSelectedObjectsActive(Object currentObject, bool value)
		{
			if (!IsCurrentObjectSelected(currentObject))
			{
				return;
			}

			var objectsSelected = Selection.gameObjects;
			for (var i = 0; i < objectsSelected.Length; i++)
			{
				objectsSelected[i].SetActive(value);
			}
		}

		private static bool IsCurrentObjectSelected(Object currentObject)
		{
			var objectsSelected = Selection.gameObjects;

			for (var i = 0; i < objectsSelected.Length; i++)
			{
				if (currentObject == objectsSelected[i])
				{
					return true;
				}
			}

			return false;
		}

		private static void MakeSceneDirty()
		{
			var activeScene = SceneManager.GetActiveScene();
			if (!EditorApplication.isPlaying)
			{
				EditorSceneManager.MarkSceneDirty(activeScene);
			}
		}
		
		private static void MakePrefabDirty()
		{
			
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null)
			{
				EditorSceneManager.MarkSceneDirty(prefabStage.scene);
			}
		}
	}
}
#endif