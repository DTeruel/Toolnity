#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity
{
	public class StoryboardCreator : MonoBehaviour
	{
		private const string DEFAULT_SCREENSHOT_NAME = "Screen";
		
		[SerializeField] private bool takeScreenshotsOnPlay;
		[SerializeField] private string screenshotName = DEFAULT_SCREENSHOT_NAME;
		[SerializeField] private int width = 2560;
		[SerializeField] private int height = 1440;
		[SerializeField] private Camera[] cameras;

		private bool takeScreenshot;

		private void Start()
		{
			takeScreenshot = takeScreenshotsOnPlay;
		}

		public void TakeScreenshots()
		{
			takeScreenshot = true;
			if (!Application.isPlaying)
			{
				LateUpdate();
			}
		}

		private void LateUpdate()
		{
			if (!takeScreenshot)
			{
				return;
			}
			
			takeScreenshot = false;
			
			if (cameras.Length == 0)
			{
				Debug.Log("[Storyboard Creator] No cameras to take screenshots.");
				return;
			}
			
			Debug.Log("[Storyboard Creator] Taking Screenshots...");
			
			CheckScreenshotName();
			for (var i = 0; i < cameras.Length; i++)
			{
				TakeScreenshot(cameras[i], i + 1);
			}

			AssetDatabase.Refresh();
		}

		private void CheckScreenshotName()
		{
			if (string.IsNullOrEmpty(screenshotName))
			{
				screenshotName = DEFAULT_SCREENSHOT_NAME;
			}
		}

		private void TakeScreenshot(Camera cameraToCapture, int index)
		{
			if (cameraToCapture == null)
			{
				return;
			}

			var rt = new RenderTexture(width, height, 24);
			cameraToCapture.targetTexture = rt;
			var screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
			cameraToCapture.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			cameraToCapture.targetTexture = null;
			RenderTexture.active = null;
			DestroyImmediate(rt);
			var bytes = screenShot.EncodeToPNG();
			var directory = GetScreenshotDirectory();
			var filename = GetScreenshotName(index);
			if (!System.IO.Directory.Exists(directory))
			{
				System.IO.Directory.CreateDirectory(directory);
			}
			System.IO.File.WriteAllBytes(directory + filename, bytes);

			Debug.Log($"[Storyboard Creator] Took screenshot: {filename}");
		}

		private string GetScreenshotDirectory()
		{
			return $"{Application.dataPath}/Storyboards/{screenshotName}/";
		}

		private string GetScreenshotName(int index)
		{
			var filename = screenshotName.Replace('/', '_');
			return $"{filename}_{index:000}.png";
		}
	}
}
#endif