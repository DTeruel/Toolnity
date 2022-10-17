#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// Based on:
/// http://wiki.unity3d.com/index.php?title=SceneAutoLoader&oldid=19980

namespace Toolnity.EditorExtensions
{
	[InitializeOnLoad]
	internal static class LoadSceneOnPlay
	{
		public static bool LoadMasterOnPlay
		{
			get => EditorExtensions.Config.SceneOnPlay;
			set => EditorExtensions.Config.SceneOnPlay = value;
		}

		public static string MasterScene
		{
			get => EditorExtensions.Config.MasterScene;
			set => EditorExtensions.Config.MasterScene = value;
		}

		private static string PreviousScene
		{
			get => EditorExtensions.Config.PreviousScene;
			set => EditorExtensions.Config.PreviousScene = value;
		}
		
		static LoadSceneOnPlay()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		public static void SelectMasterScene()
		{
			var masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
			masterScene = masterScene.Replace(Application.dataPath, "Assets");
			if (string.IsNullOrEmpty(masterScene))
			{
				LoadMasterOnPlay = false;
			}
			else
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