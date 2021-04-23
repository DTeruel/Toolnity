#if UNITY_EDITOR
using UnityEditor;

namespace Toolnity
{
	public static class PlayGame
	{
		[MenuItem("Tools/Toolnity/Shortcuts/Game Play | Stop _F5", priority = 10)]
		private static void PlayStop()
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorApplication.EnterPlaymode();
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Game Pause | Resume _F6", priority = 10)]
		private static void PauseResume()
		{
			EditorApplication.isPaused = !EditorApplication.isPaused;
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Game Next Frame _F7", priority = 10)]
		private static void NextFrame()
		{
			EditorApplication.Step();
		}
	}
}
#endif