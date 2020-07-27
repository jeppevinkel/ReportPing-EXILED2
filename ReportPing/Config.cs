using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Exiled.API.Interfaces;

namespace ReportPing
{
	public sealed class Config : IConfig
	{
		[Description("Indicates whether the plugin is enabled or not")]
		public bool IsEnabled { get; set; } = true;

		[Description("RoleID to ping")] public List<string> RoleId { get; set; } = new List<string> {"RoleID"};
	}
}
