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
		private const string CHANGE_ALL_OPTION_NAME = "Tools/Toolnity/Hierarchy Utils/Change All Selected Objects";
		private const float BUTTON_SIZE = 15;
		private const float BUTTON_OFFSET = 2 + BUTTON_SIZE * 2;

		private static bool changeAllSelectedObjects;

		static HierarchyObjectActive()
		{
			EditorApplication.delayCall += DelayCall;
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
		}

		private static void DelayCall()
		{
			changeAllSelectedObjects = EditorPrefs.GetBool(CHANGE_ALL_OPTION_NAME, true);
			Menu.SetChecked(CHANGE_ALL_OPTION_NAME, changeAllSelectedObjects);
		}

		[MenuItem(CHANGE_ALL_OPTION_NAME)]
		private static void ToggleMode()
		{
			changeAllSelectedObjects = !changeAllSelectedObjects;
			Menu.SetChecked(CHANGE_ALL_OPTION_NAME, changeAllSelectedObjects);
			EditorPrefs.SetBool(CHANGE_ALL_OPTION_NAME, changeAllSelectedObjects);
		}

		private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
		{
			var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (gameObject == null)
			{
				return;
			}

			var r = new Rect(selectionRect) {x = BUTTON_OFFSET, width = BUTTON_SIZE};
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
			if (!changeAllSelectedObjects || !IsCurrentObjectSelected(currentObject))
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