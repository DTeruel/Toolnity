#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Toolnity
{

    class ColorPickerWindow : EditorWindow
    {
        public Color color = Color.green;

        private Action<Color> setColorAction;

        public void Init(Color color, Action<Color> onSetColor)
        {
            this.color = color;
            setColorAction = onSetColor;
        }

        void OnGUI()
        {
            var oldColor = color;
            color = EditorGUILayout.ColorField(color);

            if (oldColor != color)
            {
                setColorAction?.Invoke(color);
            }
        }
    }
}
#endif
