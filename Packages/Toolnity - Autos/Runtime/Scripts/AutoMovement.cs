using UnityEngine;

namespace Toolnity
{
	public class AutoMovement : AutoBaseClass
	{
		[SerializeField] private bool moveForward;
		[Tooltip("Only used when moveForward = false")]
		[SerializeField] private Vector3 vectorToMove = Vector3.zero;
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
			if (moveForward)
			{
				transform.position += transform.forward * (speed * Time.deltaTime);
			}
			else
			{
				transform.position += vectorToMove * (speed * Time.deltaTime);
			}
		}
	}
}