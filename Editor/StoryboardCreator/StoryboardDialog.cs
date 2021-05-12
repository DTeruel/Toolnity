using UnityEngine;
using UnityEngine.UI;

namespace Toolnity
{
	public class StoryboardDialog : MonoBehaviour
	{
		[SerializeField] private Text dialogText;

		public void SetDialog(Camera cameraToDraw, string dialog)
		{
			dialogText.text = dialog;
			
			var canvas = GetComponent<Canvas>();
			canvas.worldCamera = cameraToDraw;
		}
	}
}