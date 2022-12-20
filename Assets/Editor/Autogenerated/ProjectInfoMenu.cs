#if UNITY_EDITOR
/*******************
*  AUTO GENERATED  *
/*******************/

using UnityEngine;
using UnityEditor;
using Toolnity.ProjectInfo;

public static class ProjectInfoMenu
{
	[MenuItem("Toolnity/Configure", priority = 10000)]
	private static void ConfigureLinks()
	{
		Selection.activeObject = ProjectInfo.Config;
	}

}
#endif
