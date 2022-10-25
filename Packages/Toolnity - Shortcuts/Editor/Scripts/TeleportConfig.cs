using UnityEditor;
using UnityEngine;

namespace Toolnity.Shortcuts
{
	[CreateAssetMenu(menuName = "Toolnity/Teleport Config", fileName = "Teleport Config")]
	public class TeleportConfig : ScriptableObject
	{
		[SerializeField]
		private bool copyAxisX = true;
		public bool CopyAxisX
		{
			get => copyAxisX;
			set
			{
				copyAxisX = value;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}
		}
		
		[SerializeField]
		private bool copyAxisY = true;
		public bool CopyAxisY
		{
			get => copyAxisY;
			set
			{
				copyAxisY = value;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}
		}
		
		[SerializeField]
		private bool copyAxisZ = true;
		public bool CopyAxisZ
		{
			get => copyAxisZ;
			set
			{
				copyAxisZ = value;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}
		}
	}
}
