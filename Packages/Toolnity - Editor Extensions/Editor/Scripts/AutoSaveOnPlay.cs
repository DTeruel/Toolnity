#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace Toolnity.EditorExtensions
{
	[InitializeOnLoad]
	public class AutoSaveOnPlay : EditorWindow
	{
		static AutoSaveOnPlay()
		{
			EditorApplication.playModeStateChanged += SaveProject;
		}

		private static void SaveProject(PlayModeStateChange state)
		{
			if (!EditorExtensions.SaveOnPlay || EditorApplication.isPlaying)
			{
				return;
			}

			SaveAll();
		}

		public static void SaveAll()
		{
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
				if (SaveCurrentModifiedScenesIfUserWantsTo())
				{
					Debug.Log("[Toolnity] Scenes Saved!");
				}
			}
		}
		
		private static void SavePrefab()
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

				Debug.Log("[Toolnity] Prefab Saved!");
			}
		}
		
		private static void SaveProject()
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log("[Toolnity] Project Saved!");
		}
	}
}
#endif
