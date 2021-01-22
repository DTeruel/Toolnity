using UnityEngine;

namespace Toolnity
{
	public class AutoScaleBounce : AutoBaseClass
	{
		[SerializeField] private float maxScale = 1.2f;
		[SerializeField] private float minScale = 0.8f;
		[SerializeField] private float speed = 1.0f;

		private float currentLerpValue;
		private int direction = 1;

		protected override void InitInternal()
		{
			currentLerpValue = 0;
			transform.localScale = Vector3.one * minScale;
		}

		protected override void PlayInternal()
		{
		}

		protected override void StopInternal()
		{
		}

		protected override void UpdateInternal()
		{
			currentLerpValue += direction * speed * Time.deltaTime;
			if (currentLerpValue > 1 && direction > 0)
			{
				direction = -1;
			}
			else if (currentLerpValue < 0 && direction < 0)
			{
				direction = 1;
			}

			transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, currentLerpValue);
		}
	}
}