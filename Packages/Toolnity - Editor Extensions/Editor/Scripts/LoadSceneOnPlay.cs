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
				EditorExtensions.LoadSceneOnPlay = false;
			}
			else
			{
				EditorExtensions.MasterScene = masterScene;
				EditorExtensions.LoadSceneOnPlay = true;
			}
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			if (!EditorExtensions.LoadSceneOnPlay)
			{
				return;
			}

			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// User pressed play -- autoload master scene.
				EditorExtensions.PreviousScene = SceneManager.GetActiveScene().path;
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					try
					{
						EditorSceneManager.OpenScene(EditorExtensions.MasterScene);
					}
					catch
					{
						Debug.LogError($"error: scene not found: {EditorExtensions.MasterScene}");
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
					EditorSceneManager.OpenScene(EditorExtensions.PreviousScene);
				}
				catch
				{
					Debug.LogError($"error: scene not found: {EditorExtensions.PreviousScene}");
				}
			}
		}
	}
}
#endif