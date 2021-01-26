using Sirenix.OdinInspector;
using UnityEngine;

namespace Toolnity
{
	public class AutoScaleAnim : AutoBaseClass
	{
		[SerializeField] private AnimationCurve lerpAnimation;
		[SerializeField] private float duration = 1;
		[SerializeField] private bool loop;

		private float timer;
		private float lerpValue;

		protected override void InitInternal()
		{
		}

		protected override void PlayInternal()
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
			if (lerpValue >= 1)
			{
				if (loop)
				{
					lerpValue -= 1;
				}
				else
				{
					lerpValue = 1;
					UpdateScale();
					
					Stop();
					return;
				}
			}

			UpdateScale();
			timer += Time.deltaTime;
			lerpValue = timer / duration;
		}

		private void UpdateScale()
		{
			var scale = lerpAnimation.Evaluate(lerpValue);
			gameObject.transform.localScale = Vector3.one * scale;
		}

		private void OnDisable()
		{
			Stop();
		}
	}
}