#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace Toolnity
{
	public static class SceneSelector
	{
		public const string SCENE_SELECTOR_ENABLED = "Toolnity/Scene Selector/Enabled";
		public const string SCENE_SELECTOR_JUST_SCENES_IN_BUILD = "Toolnity/Scene Selector/JustScenesInBuild";

		public static readonly List<string> NamesList = new ();
		private static readonly List<string> PathsList = new ();

		public static void OpenSceneByName(string selection)
		{
			for (var i = 0; i < NamesList.Count; i++)
			{
				var option = NamesList[i];
				if (selection == option)
				{
					AutoSave();
					OpenScene(PathsList[i]);
					return;
				}
			}
		}
		private static void AutoSave()
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
					Debug.Log("- - - - - - - - - - - - - - - - - - - - - - - SCENES SAVED - - - - - - - - - - - - - - - - - - - - - - -");
				}
			}
		}

		public static void UpdateScenes(bool justScenesInBuild)
		{
			NamesList.Clear();
			PathsList.Clear();
			
			if (justScenesInBuild)
			{
				GetScenesFromBuild();
			}
			else
			{
				GetScenesFromProject();
			}
		}

		private static void GetScenesFromBuild()
		{
			var sceneInfo = EditorBuildSettings.scenes;
			for (var i = 0; i < sceneInfo.Length; i++)
			{
				var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneInfo[i].path);
				if (scene)
				{
					NamesList.Add(scene.name);
					PathsList.Add(sceneInfo[i].path);
				}
			}
		}

		private static void GetScenesFromProject()
		{
			var guids = AssetDatabase.FindAssets("t:Scene");
			for (var i = 0; i < guids.Length; i++)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[i]);
				var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
				if (scene)
				{
					NamesList.Add(scene.name);
					PathsList.Add(path);
				}
			}
		}

		internal class SceneDropdownPostprocessor : AssetPostprocessor
		{
			private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
				string[] movedAssets, string[] movedFromAssetPaths)
			{
				var justScenesInBuild = EditorPrefs.GetBool(Application.dataPath + SCENE_SELECTOR_JUST_SCENES_IN_BUILD);
				UpdateScenes(justScenesInBuild);
			}
		}
	}
}
#endif