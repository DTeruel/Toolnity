using UnityEngine;

namespace Toolnity
{
	public class AutoDisable : AutoBaseClass
	{
		[SerializeField] private float timeToDisable = 1;

		private float time;

		protected override void InitInternal()
		{
		}

		protected override void PlayInternal()
		{
			time = 0;
		}

		protected override void StopInternal()
		{
		}

		protected override void UpdateInternal()
		{
			time += Time.deltaTime;
			if (time > timeToDisable)
			{
				Stop();
				gameObject.SetActive(false);
			}
		}
	}
}