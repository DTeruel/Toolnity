using System;
using TMPro;
using UnityEngine;

namespace Toolnity.RuntimeScripts
{
    public class FrameRateDisplay : FrameRateCounter
    {
        private enum InfoType { FPS, MS }
        [SerializeField] private InfoType infoToShow;
        [SerializeField] private TextMeshProUGUI textMesh;

        private void Start()
        {
            FPSUpdated += OnFPSUpdated;
            MilliSecondsUpdated += OnMilliSecondsUpdated;
        }

        private void OnFPSUpdated(float number)
        {
            if (infoToShow != InfoType.FPS)
            {
                return;
            }
            UpdateText("FPS: " + number.ToString("0"));
        }

        private void OnMilliSecondsUpdated(float number)
        {
            if (infoToShow != InfoType.MS)
            {
                return;
            }
            UpdateText("MS: " + number.ToString("0.00"));
        }

        private void UpdateText(string text)
        {
            textMesh.text = text;
        }
    }
}