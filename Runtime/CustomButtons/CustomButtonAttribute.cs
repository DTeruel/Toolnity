using System;

namespace Toolnity
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CustomButton : Attribute
	{
		public string Name = "";
		public string NameFunction = "";
		public string Icon = "";
		public bool ShowInRuntime = true;
	}
}
