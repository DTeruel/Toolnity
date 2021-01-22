using UnityEngine;
using UnityEngine.Events;

namespace Toolnity
{
	public class AutoDestroy : AutoBaseClass
	{
		[SerializeField] private float timeToDestroy;
		[SerializeField] private UnityEvent onDestroyEvent;
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
			if (reset)
			{
				timer = 0;
			}

			if (time >= 0)
			{
				timeToDestroy = time;
			}

			Running = true;
		}

		protected override void StopInternal()
		{
		}

		protected override void UpdateInternal()
		{
			if (timer < timeToDestroy)
			{
				timer += Time.deltaTime;
				return;
			}

			onDestroyEvent.Invoke();
			Stop();
			Destroy(gameObject);
		}
	}
}