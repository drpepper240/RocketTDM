using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDM
{
	public class CSettings
	{
		//Date-time logfile for this plugin
		public string LogFileName = @"Logs\TDM.log";

		//Csv-like logfile for kills/deaths
		public string FragLogFileName = @"Logs\TDMDeaths.log";

		//Status file location
		public string StatusFileName = @"Plugins\TDMstatus.xml";

		//Teams
		public UInt64 TeamASteamId = 103582791457241638; //http://steamcommunity.com/groups/YARR-240
		public UInt64 TeamBSteamId = 103582791457591564; //http://steamcommunity.com/groups/YARR-240-B

		//Time to start TDM (= set status.isActive to true and back again automatically)
		public DateTime startTime = DateTime.MinValue;
		public DateTime endTime = DateTime.MaxValue;
	}
}
