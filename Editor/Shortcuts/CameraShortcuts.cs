#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
	public static class CameraShortcuts
	{
		private static bool bottomFirst;
		private static bool rightFirst;
		private static bool backFirst;

		[MenuItem("Tools/Toolnity/Shortcuts/Camera Top-Bottom _F1", priority = -10)]
		public static void SwitchTopBottomCamera()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}
			
			if (bottomFirst)
			{
				ApplyCameraPosition(Quaternion.Euler(-90, 0, 0));
			}
			else
			{
				ApplyCameraPosition(Quaternion.Euler(90, 0, 0));
			}

			bottomFirst = !bottomFirst;
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Camera Left-Right _F2", priority = -10)]
		public static void SwitchLeftRightCamera()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}

			if (rightFirst)
			{
				ApplyCameraPosition(Quaternion.Euler(0, -90, 0));
			}
			else
			{
				ApplyCameraPosition(Quaternion.Euler(0, 90, 0));
			}

			rightFirst = !rightFirst;
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Camera Front-Back _F3", priority = -10)]
		public static void SwitchFrontBackCamera()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}

			if (backFirst)
			{
				ApplyCameraPosition(Quaternion.Euler(0, 180, 0));
			}
			else
			{
				ApplyCameraPosition(Quaternion.Euler(0, 0, 0));
			}

			backFirst = !backFirst;
		}

		private static void ApplyCameraPosition(Quaternion newRotation)
		{
			SceneView.lastActiveSceneView.LookAt(SceneView.lastActiveSceneView.pivot, newRotation);
			SceneView.lastActiveSceneView.Repaint();
		}

		[MenuItem("Tools/Toolnity/Shortcuts/Camera Perspective-Orthographic _F4", priority = -10)]
		public static void PerspectiveOrthographicCamera()
		{
			if (!ToolnitySettingsRegister.BasicShortcutsEnabled)
			{
				return;
			}

			SceneView.lastActiveSceneView.orthographic = !SceneView.lastActiveSceneView.orthographic;
			SceneView.lastActiveSceneView.Repaint();
		}
	}
}
#endif