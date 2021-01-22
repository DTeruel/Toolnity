using UnityEngine;

namespace Toolnity
{
    public class AutoBounce : AutoBaseClass
    {
        [SerializeField] private AnimationCurve lerpAnimation;
        [SerializeField] private float duration = 1;

        private float timer;
        private float lerpValue;
        private Vector3 initialPosition;

        private void Awake()
        {
            initialPosition = gameObject.transform.localPosition;
        }
        
        protected override void InitInternal()
        {
        }

        protected override void PlayInternal()
        {
            lerpValue = 0;
            timer = 0;
            UpdatePosition();
        }

        protected override void StopInternal()
        {
            lerpValue = 0;
            timer = 0;
            UpdatePosition();
        }

        protected override void UpdateInternal()
        {
            if (lerpValue > 1)
            {
                lerpValue -= 1;
            }

            UpdatePosition();
            timer += Time.deltaTime;
            lerpValue = timer / duration;
        }

        private void UpdatePosition()
        {
            var yPos = lerpAnimation.Evaluate(lerpValue);
            gameObject.transform.localPosition = initialPosition + Vector3.up * yPos;
        }

        private void OnDisable()
        {
            StopInternal();
        }
    }
}