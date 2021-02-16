using UnityEngine;

namespace Toolnity
{
	public class AutoScaleCurve : AutoBaseClass
	{
		[SerializeField] private AnimationCurve scaleAnimation;
		[SerializeField] private float speed = 1.0f;
		[SerializeField] private bool loop;
		
		private float currentLerpValue;

		protected override void InitInternal()
		{
			transform.localScale = Vector3.one * scaleAnimation.Evaluate(0);
		}

		protected override void PlayInternal()
		{
			currentLerpValue = 0;
		}

		protected override void StopInternal()
		{
		}

		protected override void UpdateInternal()
		{
			currentLerpValue += speed * Time.deltaTime;
			transform.localScale = Vector3.one * scaleAnimation.Evaluate(currentLerpValue);
			if (currentLerpValue > 1)
			{
				if (loop)
				{
					currentLerpValue -= 1;
				}
				else
				{
					Stop();
				}
			}
		}
	}
}