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
		public string logFileName;

		//Csv-like logfile for kills/deaths
		public string fragLogFileName;

		//Status file location
		public string statusFileName;

		//Teams
		public string teamASteamUri;
		public UInt64 teamASteamId;

		public string teamBSteamUri;
		public UInt64 teamBSteamId;

		//autokick for plauers not belonging to either Team A or Team B
		public bool kickPlayerWithInvalidTeam;
		public UInt32 kickDelaySeconds;

		//Time to start TDM (= set status.isActive to true and back again automatically)
		public DateTime startTime;
		public DateTime endTime;

		public UnityEngine.Color messageColor;
		public UnityEngine.Color scoreColor;


		public void LoadDefaults()
		{
			logFileName = @"Logs\TDM.log";
			fragLogFileName = @"Logs\TDMDeaths.log";
			statusFileName = @"Plugins\TDMstatus.xml";

			teamASteamUri = @"http://steamcommunity.com/groups/YARR-240";
			teamASteamId = 103582791457241638;
			teamBSteamUri = @"http://steamcommunity.com/groups/YARR-240-B";
			teamBSteamId = 103582791457591564;

			kickPlayerWithInvalidTeam = true;
			kickDelaySeconds = 10;

			startTime = DateTime.MinValue;
			endTime = DateTime.MaxValue;

			messageColor = new UnityEngine.Color(0.18f, 0.8f, 0.44f, 0);
			scoreColor = new UnityEngine.Color(1, 0, 0.5f, 0);
		}
	}
}
