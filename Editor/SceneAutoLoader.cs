#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// Original file from:
/// http://wiki.unity3d.com/index.php?title=SceneAutoLoader&oldid=19980

namespace Toolnity
{
	/// <summary>
	/// Scene auto loader.
	/// </summary>
	/// <description>
	/// This class adds a File > Scene Autoload menu containing options to select
	/// a "master scene" enable it to be auto-loaded when the user presses play
	/// in the editor. When enabled, the selected scene will be loaded on play,
	/// then the original scene will be reloaded on stop.
	///
	/// Based on an idea on this thread:
	/// http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
	/// </description>
	[InitializeOnLoad]
	internal static class SceneAutoLoader
	{
		private const string LOAD_MASTER_OPTION_NAME = "Tools/Toolnity/Scene Autoload/Load Master On Play";
		
		static SceneAutoLoader()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		[MenuItem("Tools/Toolnity/Scene Autoload/Select Master Scene...")]
		private static void SelectMasterScene()
		{
			var masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
			masterScene = masterScene.Replace(Application.dataPath, "Assets");
			if (!string.IsNullOrEmpty(masterScene))
			{
				MasterScene = masterScene;
				LoadMasterOnPlay = EditorPrefs.GetBool(LOAD_MASTER_OPTION_NAME, true);
				if (Menu.GetChecked(LOAD_MASTER_OPTION_NAME))
				{
					Menu.SetChecked(LOAD_MASTER_OPTION_NAME, LoadMasterOnPlay);
				}
			}
		}

		[MenuItem(LOAD_MASTER_OPTION_NAME + " _F8")]
		private static void ToggleLoadMasterOnPlay()
		{
			LoadMasterOnPlay = !LoadMasterOnPlay;
			Menu.SetChecked(LOAD_MASTER_OPTION_NAME, LoadMasterOnPlay);
			EditorPrefs.SetBool(LOAD_MASTER_OPTION_NAME, LoadMasterOnPlay);
			
			Debug.Log("[Toolnity] Load Master On Play: " + LoadMasterOnPlay);
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			if (!LoadMasterOnPlay)
			{
				return;
			}

			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// User pressed play -- autoload master scene.
				PreviousScene = SceneManager.GetActiveScene().path;
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					try
					{
						EditorSceneManager.OpenScene(MasterScene);
					}
					catch
					{
						Debug.LogError($"error: scene not found: {MasterScene}");
						EditorApplication.isPlaying = false;

					}
				}
				else
				{
					// User cancelled the save operation -- cancel play as well.
					EditorApplication.isPlaying = false;
				}
			}

			// isPlaying check required because cannot OpenScene while playing
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// User pressed stop -- reload previous scene.
				try
				{
					EditorSceneManager.OpenScene(PreviousScene);
				}
				catch
				{
					Debug.LogError($"error: scene not found: {PreviousScene}");
				}
			}
		}

		private const string C_EDITOR_PREF_LOAD_MASTER_ON_PLAY = "SceneAutoLoader.LoadMasterOnPlay";
		private const string C_EDITOR_PREF_MASTER_SCENE = "SceneAutoLoader.MasterScene";
		private const string C_EDITOR_PREF_PREVIOUS_SCENE = "SceneAutoLoader.PreviousScene";

		private static bool LoadMasterOnPlay
		{
			get => EditorPrefs.GetBool(C_EDITOR_PREF_LOAD_MASTER_ON_PLAY, false);
			set => EditorPrefs.SetBool(C_EDITOR_PREF_LOAD_MASTER_ON_PLAY, value);
		}

		private static string MasterScene
		{
			get => EditorPrefs.GetString(C_EDITOR_PREF_MASTER_SCENE, "Master.unity");
			set => EditorPrefs.SetString(C_EDITOR_PREF_MASTER_SCENE, value);
		}

		private static string PreviousScene
		{
			get => EditorPrefs.GetString(C_EDITOR_PREF_PREVIOUS_SCENE, SceneManager.GetActiveScene().path);
			set => EditorPrefs.SetString(C_EDITOR_PREF_PREVIOUS_SCENE, value);
		}
	}
}
#endif