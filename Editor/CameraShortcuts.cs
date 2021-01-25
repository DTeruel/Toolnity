#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
	public class CameraShortcuts : MonoBehaviour
	{
		private static bool bottomFirst;
		private static bool rightFirst;
		private static bool backFirst;

		[MenuItem("Tools/Toolnity/Camera/Top-Bottom _F1")]
		private static void SwitchTopBottomCamera()
		{
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

		[MenuItem("Tools/Toolnity/Camera/Left-Right _F2")]
		private static void SwitchLeftRightCamera()
		{
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

		[MenuItem("Tools/Toolnity/Camera/Front-Back _F3")]
		private static void SwitchFrontBackCamera()
		{
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

		[MenuItem("Tools/Toolnity/Camera/Perspective-Orthographic _F4")]
		private static void PerspectiveOrthographicCamera()
		{
			SceneView.lastActiveSceneView.orthographic = !SceneView.lastActiveSceneView.orthographic;
			SceneView.lastActiveSceneView.Repaint();
		}
	}
}
#endif