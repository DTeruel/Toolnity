using UnityEngine;

namespace Toolnity
{
	public class AutoFollow : AutoBaseClass
	{
		[SerializeField] private Transform target;
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
			transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
		}
	}
}