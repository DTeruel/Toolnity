#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorUtils
{
	[InitializeOnLoad]
	public class SceneSelector : EditorWindow
	{
		private const string ACTIVE_OPTION_NAME = "Tools/Toolnity/Scene Selector/Active";
		private const string JUST_SCENES_IN_BUILD_OPTION_NAME = "Tools/Toolnity/Scene Selector/Use Scenes just in build";

		private static int sceneNameIndex;
		private static bool active;
		private static bool justScenesInBuild;
		private static bool showSceneLauncher;
		private static readonly List<string> NamesList = new List<string>();
		private static readonly List<string> PathsList = new List<string>();
		private static string buttonText;

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
			
			EditorSceneManager.sceneOpened += SceneOpenedCallback;
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
		}

		private static void OnSceneGUI(SceneView sceneView)
		{
			if (!active)
			{
				return;
			}
			
			Handles.BeginGUI();
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button(buttonText))
			{
				showSceneLauncher = !showSceneLauncher;
			}
			
			if (showSceneLauncher)
			{
				buttonText = "<";

				var newJustScenesInBuild = GUILayout.Toggle(justScenesInBuild, "");
				if (justScenesInBuild != newJustScenesInBuild)
				{
					ToggleJustScenesInBuild();
				}

				var newSelection = EditorGUILayout.Popup(sceneNameIndex, NamesList.ToArray());
				if (sceneNameIndex != newSelection)
				{
					sceneNameIndex = newSelection;
					EditorSceneManager.OpenScene(PathsList[sceneNameIndex]);
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

		private static void SceneOpenedCallback(Scene scene, OpenSceneMode mode)
		{
			if (mode != OpenSceneMode.Single)
			{
				return;
			}

			CheckSceneIndex(scene);
		}

		private static void CheckSceneIndex(Scene scene)
		{
			for (var i = 0; i < NamesList.Count; i++)
			{
				if (NamesList[i] == scene.name)
				{
					sceneNameIndex = i;
					return;
				}
			}
		}

		public static void UpdateScenes()
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

			if (sceneNameIndex >= NamesList.Count)
			{
				sceneNameIndex = 0;
			}

			CheckSceneIndex(SceneManager.GetActiveScene());
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