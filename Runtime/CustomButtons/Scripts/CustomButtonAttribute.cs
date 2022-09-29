using System;

namespace Toolnity
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CustomButton : Attribute
	{
		public string Name = "";
		public string NameFunction = "";
		public bool NameFunctionCalledJustOnce = true;
		public string Icon = "";
		public bool ShowInRuntime = true;
		public bool UseClassNameAsPath = true;
		public bool UseGameObjectNameAsPath = true;
		public bool CloseMenuOnPressed = false;
	}
}
