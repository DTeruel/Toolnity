using UnityEngine;

namespace Toolnity
{
	public abstract class AutoBaseClass : MonoBehaviour
	{
		[SerializeField] private bool startActivated;

		protected bool Running;
		protected bool Initiated;

		protected abstract void InitInternal();

		protected abstract void PlayInternal();

		protected abstract void StopInternal();

		protected abstract void UpdateInternal();

		private void Start()
		{
			if (Initiated)
			{
				return;
			}
			
			InitInternal();
			Initiated = true;
			
			if (startActivated)
			{
				Play();
			}
		}

		public void Play()
		{
			if (!Initiated)
			{
				Start();
			}
			
			Running = true;
			PlayInternal();
		}

		public void Stop()
		{
			Running = false;
			StopInternal();
		}

		private void Update()
		{
			if (!Running)
			{
				return;
			}

			UpdateInternal();
		}
	}
}