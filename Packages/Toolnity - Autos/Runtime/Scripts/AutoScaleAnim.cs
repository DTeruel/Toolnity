using UnityEngine;

namespace Toolnity
{
	public class AutoScaleAnim : AutoBaseClass
	{
		[SerializeField] private AnimationCurve lerpAnimation;
		[SerializeField] private float duration = 1;
		[SerializeField] private bool loop;
		[SerializeField] private bool canBeInterrupted = true;
		[SerializeField] private bool resetOnEnable = true;

		private float timer;
		private float lerpValue;

		private void OnEnable()
		{
			if (resetOnEnable)
			{
				Reset();
			}
		}

		protected override void InitInternal()
		{
		}

		protected override void PlayInternal()
		{
			if (Running && !canBeInterrupted)
			{
				return;
			}
			
			Reset();
		}
		
		public void Reset()
		{
			lerpValue = 0;
			timer = 0;
			UpdateScale();
		}

		protected override void StopInternal()
		{
		}

		protected override void UpdateInternal()
		{
			UpdateScale();
			UpdateTimer();
		}

		private void UpdateScale()
		{
			var scale = lerpAnimation.Evaluate(lerpValue);
			gameObject.transform.localScale = Vector3.one * scale;
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
					UpdateScale();
					Stop();
					return;
				}
			}
			
			lerpValue = timer / duration;
		}
	}
}