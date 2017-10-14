using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace TDM
{
	public class Configuration : IRocketPluginConfiguration
	{
		//Date-time logfile for this plugin
		public string LogFileName;

		//Csv-like logfile for kills/deaths
		public string FragLogFileName;

		//Status file location
		public string StatusFileName;

		//Teams
		public string TeamASteamUri;
		public UInt64 TeamASteamId;

		public string TeamBSteamUri;
		public UInt64 TeamBSteamId;

		//autokick for plauers not belonging to either Team A or Team B
		public bool kickPlayerWithInvalidTeam;
		public UInt32 kickDelaySeconds;

		//Time to start TDM (= set status.isActive to true and back again automatically)
		public DateTime startTime;
		public DateTime endTime;


		public void LoadDefaults()
		{
			LogFileName = @"Logs\TDM.log";
			FragLogFileName = @"Logs\TDMDeaths.log";
			StatusFileName = @"Plugins\TDMstatus.xml";

			TeamASteamUri = @"http://steamcommunity.com/groups/YARR-240";
			TeamASteamId = 103582791457241638;
			TeamBSteamUri = @"http://steamcommunity.com/groups/YARR-240-B";
			TeamBSteamId = 103582791457591564;

			kickPlayerWithInvalidTeam = true;
			kickDelaySeconds = 10;

			startTime = DateTime.MinValue;
			endTime = DateTime.MaxValue;
		}
	}
}
