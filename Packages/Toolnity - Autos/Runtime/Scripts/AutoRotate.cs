using UnityEngine;

namespace Toolnity
{
	public class AutoRotate : AutoBaseClass
	{
		[SerializeField] private Vector3 vectorToRotate = Vector3.zero;
		[SerializeField] private float speed = 1.0f;

		
		protected override void InitInternal()
		{
		}

		protected override void PlayInternal()
		{
		}

		protected override void StopInternal()
		{
		}
		
		protected override void UpdateInternal()

		{
			transform.Rotate(vectorToRotate, speed * Time.deltaTime);
		}
	}
}