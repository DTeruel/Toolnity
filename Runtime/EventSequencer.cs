using System;
using UnityEngine;
using UnityEngine.Events;

namespace Toolnity
{
    public class EventSequencer : MonoBehaviour
    {
        [Serializable]
        private struct SequenceStep
        {
            public float timeToStart;
            public UnityEvent eventToTrigger;
        }

        [SerializeField] private bool playAtStart;
        [SerializeField] private SequenceStep[] sequence;

        private float timer;
        private bool running;
        private int currentIndex;

        private void Start()
        {
            if (playAtStart)
            {
                Play();
            }
        }

        public void Play()
        {
            if (sequence.Length == 0)
            {
                return;
            }

            running = true;
            currentIndex = 0;
            timer = 0;
        }

        private void Stop()
        {
            running = false;
        }

        private void Update()
        {
            if (!running)
            {
                return;
            }

            if (timer < sequence[currentIndex].timeToStart)
            {
                timer += Time.deltaTime;
            }
            else
            {
                sequence[currentIndex].eventToTrigger.Invoke();
                currentIndex++;
                timer = 0;

                if (currentIndex == sequence.Length)
                {
                    Stop();
                }
            }
        }
    }
}