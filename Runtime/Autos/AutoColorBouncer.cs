using UnityEngine;
using UnityEngine.UI;

namespace Toolnity
{
    public class AutoColorBouncer : AutoBaseClass
    {
        [SerializeField] private Image image;
        [SerializeField] private Color colorStart;
        [SerializeField] private Color colorEnd;
        [SerializeField] private AnimationCurve lerpAnimation;
        [SerializeField] private float speed = 1;

        private float lerpValue;

        protected override void InitInternal()
        {
        }

        protected override void PlayInternal()
        {
            Reset();
        }

        protected override void StopInternal()
        {
        }

        protected override void UpdateInternal()
        {
            image.color = Color.Lerp(colorStart, colorEnd, lerpAnimation.Evaluate(lerpValue));
            lerpValue += speed * Time.deltaTime;

            if (lerpValue > 1)
            {
                lerpValue -= 1;
            }
        }

        public void Reset()
        {
            lerpValue = 0;
            image.color = Color.Lerp(colorStart, colorEnd, lerpAnimation.Evaluate(lerpValue));
        }
    }
}