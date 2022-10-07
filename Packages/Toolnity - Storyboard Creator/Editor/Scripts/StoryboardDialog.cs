using UnityEngine;
using UnityEngine.UI;

namespace Toolnity.StoryboardCreator
{
	public class StoryboardDialog : MonoBehaviour
	{
		[SerializeField] private Text dialogText;

		public void SetDialog(Camera cameraToRender, string dialog)
		{
			dialogText.text = dialog;
			
			var canvas = GetComponent<Canvas>();
			canvas.worldCamera = cameraToRender;
			canvas.planeDistance = cameraToRender.nearClipPlane;
		}
	}
}