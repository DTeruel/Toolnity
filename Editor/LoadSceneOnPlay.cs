﻿#if UNITY_EDITOR
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
	/// This class adds a menu containing options to select
	/// a "master scene" enable it to be auto-loaded when the user presses play
	/// in the editor. When enabled, the selected scene will be loaded on play,
	/// then the original scene will be reloaded on stop.
	///
	/// Based on an idea on this thread:
	/// http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
	/// </description>
	[InitializeOnLoad]
	internal static class LoadSceneOnPlay
	{
		public const string LOAD_SCENE_ON_PLAY_SETTINGS_ENABLED = "Toolnity/Load Scene On Play/Enabled";

		private const string EditorPrefMasterScene = "LoadSceneOnPlay.MasterScene";
		private const string EditorPrefPreviousScene = "LoadSceneOnPlay.PreviousScene";

		private static bool LoadMasterOnPlay
		{
			get => EditorPrefs.GetBool(Application.dataPath + LOAD_SCENE_ON_PLAY_SETTINGS_ENABLED, false);
			set => EditorPrefs.SetBool(Application.dataPath + LOAD_SCENE_ON_PLAY_SETTINGS_ENABLED, value);
		}

		public static string MasterScene
		{
			get => EditorPrefs.GetString(Application.dataPath + EditorPrefMasterScene, "Master.unity");
			set => EditorPrefs.SetString(Application.dataPath + EditorPrefMasterScene, value);
		}

		private static string PreviousScene
		{
			get => EditorPrefs.GetString(Application.dataPath + EditorPrefPreviousScene, SceneManager.GetActiveScene().path);
			set => EditorPrefs.SetString(Application.dataPath + EditorPrefPreviousScene, value);
		}
		
		static LoadSceneOnPlay()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		public static void SelectMasterScene()
		{
			var masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
			masterScene = masterScene.Replace(Application.dataPath, "Assets");
			if (!string.IsNullOrEmpty(masterScene))
			{
				MasterScene = masterScene;
				LoadMasterOnPlay = true;
			}
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
	}
}
#endif