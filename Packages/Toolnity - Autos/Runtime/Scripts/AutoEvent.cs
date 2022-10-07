using UnityEngine;
using UnityEngine.Events;

namespace Toolnity
{
	public class AutoEvent : AutoBaseClass
	{
		[SerializeField] private UnityEvent eventOnTime;
		[SerializeField] private float timeToLaunchEvent;

		private float timer;
		
		protected override void InitInternal()
		{
		}

		protected override void PlayInternal()
		{
			Play(true, -1);
		}

		public void Play(bool reset, float time)
		{
			Running = true;
			
			if (reset)
			{
				timer = 0;
			}

			if (time >= 0)
			{
				timeToLaunchEvent = time;
			}
		}

		protected override void StopInternal()
		{
		}

		protected override void UpdateInternal()
		{
			if (timer < timeToLaunchEvent)
			{
				timer += Time.deltaTime;
				return;
			}

			eventOnTime.Invoke();
			Stop();
		}
	}
}