using UnityEngine;

namespace Toolnity.Shortcuts
{
	[CreateAssetMenu(menuName = "Toolnity/Teleport Config", fileName = "Teleport Config")]
	public class TeleportConfig : ScriptableObject
	{
		public bool CopyAxisX = true;
		public bool CopyAxisY = true;
		public bool CopyAxisZ = true;
	}
}
