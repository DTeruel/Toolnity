#if UNITY_EDITOR
/*******************
*  AUTO GENERATED  *
/*******************/

using UnityEngine;
using UnityEditor;
using Toolnity.ProjectInfo;

public static class ProjectInfoMenu
{
	[MenuItem("Toolnity /Configure", priority = 10000)]
	private static void ConfigureLinks()
	{
		Selection.activeObject = ProjectInfo.Config;
	}

	[MenuItem("Toolnity /Links/Explorer", priority = 9000)]
	private static void OpenURL0()
	{
		Application.OpenURL("C:");
	}

	[MenuItem("Toolnity /Links/Unity", priority = 9001)]
	private static void OpenURL1()
	{
		Application.OpenURL("www.unity.com");
	}
}
#endif
