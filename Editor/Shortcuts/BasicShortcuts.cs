#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolnity
{
	public static class BasicShortcuts
	{
		[MenuItem("Tools/Toolnity/Shortcuts/Game Play-Stop _F5", priority = 10)]
		private static void PlayStop()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}
			
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorApplication.EnterPlaymode();
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Game Pause-Resume _F6", priority = 10)]
		private static void PauseResume()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}

			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Save All _F12", priority = 100)]
		public static void SaveAll()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}

			if (EditorApplication.isPlaying)
			{
				return;
			}

			SaveScenes();
			SavePrefab();
			SaveProject();
		}

		public static void SaveScenes()
		{
			var scenesWithChanges = false;
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.isDirty)
				{
					scenesWithChanges = true;
				}
			}

			if (scenesWithChanges)
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					Debug.Log("-------------------------------- SCENES SAVED --------------------------------");
				}
			}
		}
		
		public static void SavePrefab()
		{
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null)
			{
				var prefabRoot = prefabStage.prefabContentsRoot;
				var path = prefabStage.assetPath;

				try
				{
					PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
				}
				catch
				{
					// ignored
				}
				prefabStage.ClearDirtiness();

				Debug.Log("-------------------------------- PREFAB SAVED --------------------------------");
			}
		}
		
		public static void SaveProject()
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log("+++++++++++++++++++++++++ PROJECT SAVED +++++++++++++++++++++++++");
		}
	}
}
#endif