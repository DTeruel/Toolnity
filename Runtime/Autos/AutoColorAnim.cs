using UnityEngine;
using UnityEngine.UI;

namespace Toolnity
{
	public class AutoColorAnim : AutoBaseClass
	{
		[SerializeField] private Image image;
		[SerializeField] private Color colorStart;
		[SerializeField] private Color colorEnd;
		[SerializeField] private AnimationCurve lerpAnimation;
		[SerializeField] private float duration = 1;
		[SerializeField] private bool loop = true;

		private float timer;
		private float lerpValue;

		private void OnEnable()
		{
			Reset();
		}
		
		protected override void InitInternal()
		{
		}

		protected override void PlayInternal()
		{
			Reset();
		}
		
		public void Reset()
		{
			lerpValue = 0;
			timer = 0;
			UpdateColor();
		}

		protected override void StopInternal()
		{
		}

		protected override void UpdateInternal()
		{
			UpdateColor();
			UpdateTimer();
		}

		private void UpdateColor()
		{
			image.color = Color.Lerp(colorStart, colorEnd, lerpAnimation.Evaluate(lerpValue));
		}

		private void UpdateTimer()
		{
			timer += Time.deltaTime;
			if (timer > duration)
			{
				if (loop)
				{
					timer -= duration;
				}
				else
				{
					timer = duration;
					lerpValue = 1;
					UpdateColor();
					Stop();
					return;
				}
			}
			
			lerpValue = timer / duration;
		}
	}
}