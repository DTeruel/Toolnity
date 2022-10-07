using UnityEngine;

namespace Toolnity
{
	public class AutoBounce : AutoBaseClass
	{
		[SerializeField] private AnimationCurve lerpAnimation;
		[SerializeField] private float duration = 1;

		private float timer;
		private float lerpValue;
		private float initialPositionInY;

		private void Awake()
		{
			initialPositionInY = gameObject.transform.localPosition.y;
		}
		
		protected override void InitInternal()
		{
		}

		protected override void PlayInternal()
		{
			lerpValue = 0;
			timer = 0;
			UpdatePosition();
		}

		protected override void StopInternal()
		{
			lerpValue = 0;
			timer = 0;
			UpdatePosition();
		}

		protected override void UpdateInternal()
		{
			if (lerpValue > 1)
			{
				lerpValue -= 1;
			}

			UpdatePosition();
			timer += Time.deltaTime;
			lerpValue = timer / duration;
		}

		private void UpdatePosition()
		{
			var yPos = lerpAnimation.Evaluate(lerpValue);
			var currentLocalPosition = gameObject.transform.localPosition;
			currentLocalPosition.y = initialPositionInY + yPos;
			gameObject.transform.localPosition = currentLocalPosition;
		}

		private void OnDisable()
		{
			StopInternal();
		}
	}
}