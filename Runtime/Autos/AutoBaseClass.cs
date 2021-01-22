using UnityEngine;

namespace Toolnity
{
	public abstract class AutoBaseClass : MonoBehaviour
	{
		[SerializeField] private bool startActivated;

		protected bool Running;

		protected abstract void InitInternal();

		protected abstract void PlayInternal();

		protected abstract void StopInternal();

		protected abstract void UpdateInternal();

		private void Start()
		{
			Running = false;

			InitInternal();

			if (startActivated)
			{
				Play();
			}
		}

		public void Play()
		{
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