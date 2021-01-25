#if UNITY_EDITOR
using UnityEditor;

namespace Toolnity
{
	public static class PlayGame
	{
		[MenuItem("Tools/Toolnity/Play Game/Play | Stop _F5")]
		private static void PlayStop()
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorApplication.EnterPlaymode();
		}

		[MenuItem("Tools/Toolnity/Play Game/Pause | Resume _F6")]
		private static void PauseResume()
		{
			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

		[MenuItem("Tools/Toolnity/Play Game/Next Frame _F7")]
		private static void NextFrame()
		{
			EditorApplication.Step();
		}
	}
}
#endif