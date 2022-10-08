using System;
using UnityEngine;

namespace Toolnity.RuntimeScripts
{
	public class FrameRateCounter : MonoBehaviour
	{
		public event Action<float> FPSUpdated;
		public event Action<float> MilliSecondsUpdated;

		[SerializeField, Range(0.01f, 2f)] private float sampleDuration = 0.1f;

		private int frames;
		private float duration = float.MaxValue;

		private void Update()
		{
			var frameDuration = Time.unscaledDeltaTime;
			frames++;
			duration += frameDuration;

			if (duration >= sampleDuration) 
			{
				FPSUpdated?.Invoke(frames / duration);
				MilliSecondsUpdated?.Invoke(1000f * duration / frames);
				
				frames = 0;
				duration = 0f;
			}
		}
	}
}