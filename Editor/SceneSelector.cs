#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace Toolnity
{
	[InitializeOnLoad]
	public static class SceneSelector
	{
		private const string ACTIVE_OPTION_NAME = "Tools/Toolnity/Scene Selector/Active";
		private const string JUST_SCENES_IN_BUILD_OPTION_NAME = "Tools/Toolnity/Scene Selector/Search just scenes in build";

		private static bool active;
		private static bool justScenesInBuild;
		private static bool showSceneLauncher;
		private static readonly List<string> NamesList = new List<string>();
		private static readonly List<string> PathsList = new List<string>();
		private static string buttonText;

		private static GUIStyle popupMiddleAlignment;

		static SceneSelector()
		{
			active = EditorPrefs.GetBool(ACTIVE_OPTION_NAME, true);
			if (Menu.GetChecked(ACTIVE_OPTION_NAME))
			{
				Menu.SetChecked(ACTIVE_OPTION_NAME, active);
			}

			justScenesInBuild = EditorPrefs.GetBool(JUST_SCENES_IN_BUILD_OPTION_NAME, false);
			if (Menu.GetChecked(JUST_SCENES_IN_BUILD_OPTION_NAME))
			{
				Menu.SetChecked(JUST_SCENES_IN_BUILD_OPTION_NAME, justScenesInBuild);
			}

			UpdateScenes();
			
			SceneView.duringSceneGui += OnSceneGUI;
		}
		
		[MenuItem(ACTIVE_OPTION_NAME)]
		public static void ToggleActive()
		{
			active = !active;
			Menu.SetChecked(ACTIVE_OPTION_NAME, active);
			EditorPrefs.SetBool(ACTIVE_OPTION_NAME, active);

			UpdateScenes();
		}
		
		[MenuItem(JUST_SCENES_IN_BUILD_OPTION_NAME)]
		private static void ToggleJustScenesInBuild()
		{
			justScenesInBuild = !justScenesInBuild;
			Menu.SetChecked(JUST_SCENES_IN_BUILD_OPTION_NAME, justScenesInBuild);
			EditorPrefs.SetBool(JUST_SCENES_IN_BUILD_OPTION_NAME, justScenesInBuild);

			UpdateScenes();
			
			Debug.Log("[Toolnity] Search just scenes in build settings : " + justScenesInBuild);
		}

		private static void OnSceneGUI(SceneView sceneView)
		{
			if (!active)
			{
				return;
			}
			
			if (popupMiddleAlignment == null)
			{
				popupMiddleAlignment = GUI.skin.GetStyle("Popup");
				popupMiddleAlignment.alignment = TextAnchor.MiddleCenter;
				popupMiddleAlignment.fontSize = 12;
			}

			Handles.BeginGUI();
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button(buttonText))
			{
				showSceneLauncher = !showSceneLauncher;
				UpdateScenes();
			}
			
			if (showSceneLauncher)
			{
				buttonText = "<";

				var newSelection = EditorGUILayout.Popup(0, NamesList.ToArray(), popupMiddleAlignment);
				if (newSelection > 0)
				{
					AutoSave();
					OpenScene(PathsList[newSelection]);
				}

				var newJustScenesInBuild = GUILayout.Toggle(justScenesInBuild, "");
				if (justScenesInBuild != newJustScenesInBuild)
				{
					ToggleJustScenesInBuild();
				}
			}
			else
			{
				buttonText = ">";
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			Handles.EndGUI();
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

		public static void UpdateScenes()
		{
			NamesList.Clear();
			PathsList.Clear();
			NamesList.Add("- Select Scene - ");
			PathsList.Add(" ");
			
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
	}

	internal class SceneDropdownPostprocessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
			string[] movedAssets, string[] movedFromAssetPaths)
		{
			SceneSelector.UpdateScenes();
		}
	}
}
#endif