using UnityEngine;
using UnityEngine.UI;

namespace Toolnity
{
	public class Fader : MonoBehaviour
	{
		[SerializeField] private RawImage image;
		[SerializeField] private float fadeInDuration = 1f;
		[SerializeField] private float fadeOutDuration = 1f;
		[SerializeField] private Color fullColor;

		private bool running;
		private Color startColor;
		private Color endColor;
		private float lerpValue;
		private float duration;

		public void SetColor(Color newColor)
		{
			startColor = newColor;
			endColor = newColor;
		}

		public void SetDuration(float newFadeInDuration, float newFadeOutDuration)
		{
			fadeInDuration = newFadeInDuration;
			fadeOutDuration = newFadeOutDuration;
		}

		public void FadeOut()
		{
			running = true;
			lerpValue = 0;
			startColor = fullColor;
			startColor.a = 0;
			endColor = fullColor;
			duration = fadeOutDuration;
			UpdateColor();
		}

		public void FadeIn()
		{
			running = true;
			lerpValue = 0;
			startColor = image.color;
			startColor = fullColor;
			endColor = fullColor;
			endColor.a = 0;
			duration = fadeInDuration;
			UpdateColor();
		}

		private void Update()
		{
			if (!running)
			{
				return;
			}
			UpdateColor();
		}
		
		private void UpdateColor()
		{
			lerpValue += Time.deltaTime / duration;

			if (lerpValue > 1)
			{
				lerpValue = 1;
				running = false;
			}

			image.color = Color.Lerp(startColor, endColor, lerpValue);
		}
	}
}