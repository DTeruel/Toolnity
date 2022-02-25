using System;

namespace Toolnity
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CustomButton : Attribute
	{
		public string Name = "";
		public string Icon = "";
	}
}
