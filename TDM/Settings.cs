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
		public string TeamASteamUri = @"http://steamcommunity.com/groups/YARR-240";
		public UInt64 TeamASteamId = 103582791457241638;

		public string TeamBSteamUri = @"http://steamcommunity.com/groups/YARR-240-B";
		public UInt64 TeamBSteamId = 103582791457591564;
		
		public bool kickPlayerWithInvalidTeam = true;
		public UInt32 kickDelaySeconds = 10;

		//Time to start TDM (= set status.isActive to true and back again automatically)
		public DateTime startTime = DateTime.MinValue;
		public DateTime endTime = DateTime.MaxValue;
	}
}
