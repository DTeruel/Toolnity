using System;

namespace Toolnity
{
	public enum NameFunctionExecType
	{
		OnPressed = 0,
		OnCreation = 1
	}
	
	[AttributeUsage(AttributeTargets.Method)]
	public class CustomButton : Attribute
	{
		public string Name = "";
		public string NameFunction = "";
		public NameFunctionExecType NameFunctionExecType = NameFunctionExecType.OnPressed;
		
		public string Path = "";
		public bool PathAddClassName = true;
		public bool PathAddGameObjectName = true;
		
		public string Icon = "";
		public bool ShowInRuntime = true;
		public bool CloseMenuOnPressed = false;
	}
}
