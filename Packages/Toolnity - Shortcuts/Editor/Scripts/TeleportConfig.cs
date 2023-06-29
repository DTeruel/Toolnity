#if UNITY_EDITOR
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
				EditorUtility.SetDirty(this);
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
				EditorUtility.SetDirty(this);
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
				EditorUtility.SetDirty(this);
			}
		}
	}
}
#endif
