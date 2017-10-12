using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;


using Rocket.Unturned.Chat;
using Steamworks;
using Rocket.Unturned.Player;
using Rocket.Unturned.Events;
using System.IO;

/// <summary>
/// Basic Team Deathmatch plugin for Rocket https://github.com/RocketMod/Rocket
/// </summary>

namespace TDM
{
	public class TDM : RocketPlugin
	{
		public CSettings settings;	//initialized in LoadSettings
		public CStatus status;      //initialized in LoadStatus
		public DateTime lastCalled;

		public static TDM instance;

		protected override void Load()
		{
			instance = this;
			lastCalled = DateTime.Now;
			LoadSettings();
			LoadStatus();
			LogThis(DateTime.Now.ToString("s") + " TDM Loaded", settings.LogFileName);
			UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;

			/*XmlSerializer xs = new XmlSerializer(typeof(CSettings));
			TextWriter tw = new StreamWriter(@"Plugins\TDMsettings.xml");
			xs.Serialize(tw, settings);
			tw.Flush();
			tw.Close();*/
		}

		protected override void Unload()
		{
			LogThis(DateTime.Now.ToString("s") + " TDM Unloaded", settings.LogFileName);
			UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
			instance = null;
			settings = null;
			status = null;
		}

		private void UnturnedPlayerEvents_OnPlayerDeath(UnturnedPlayer player, SDG.Unturned.EDeathCause cause, SDG.Unturned.ELimb limb, CSteamID murderer)
		{
			try
			{
				LogThis(DateTime.Now.ToString("s") + ";" + player.CSteamID.ToString() + ";" + player.SteamGroupID.ToString() + ";" + cause.ToString() + ";" + murderer.ToString() + ";", settings.FragLogFileName);

				if (!status.isActive)
					return;

				if (player.SteamGroupID.m_SteamID == settings.TeamASteamId && status.isActive)
				{
					status.teamBScore += 1;
					SaveStatus();
					UnturnedChat.Say("SCORE: " + status.teamAScore.ToString() + " : " + status.teamBScore.ToString() + " - Team B scored a point!");
				}
				if (player.SteamGroupID.m_SteamID == settings.TeamBSteamId && status.isActive)
				{
					status.teamAScore += 1;
					SaveStatus();
					UnturnedChat.Say("SCORE: " + status.teamAScore.ToString() + " : " + status.teamBScore.ToString() + " - Team A scored a point!");
				}
			}
			catch (Exception ex)
			{
				Logger.LogException(ex, ex.Message);
			}
		}

		public void SaveStatus()
		{
			XmlSerializer xs = new XmlSerializer(typeof(CStatus));
			TextWriter tw = new StreamWriter(settings.StatusFileName);
			xs.Serialize(tw, status);
			tw.Flush();
			tw.Close();
		}

		public void LoadStatus()
		{
			XmlSerializer xs = new XmlSerializer(typeof(CStatus));
			try
			{
				StreamReader sr = new StreamReader(settings.StatusFileName);
				status = (CStatus)xs.Deserialize(sr);
				sr.Close();
			} catch (Exception ex)
			{
				Logger.LogException(ex, ex.Message);
			}
			
			if (status == null)
			{
				LogThis(DateTime.Now.ToString("s") + " No status loaded - using default", settings.LogFileName);
				status = new CStatus();
				SaveStatus();
			}
			
		}

		public void LoadSettings()
		{
			XmlSerializer xs = new XmlSerializer(typeof(CSettings));
			try
			{
				StreamReader sr = new StreamReader(@"Plugins\TDMsettings.xml");
				settings = (CSettings)xs.Deserialize(sr);
				sr.Close();
			}
			catch (Exception ex)
			{
				Logger.LogException(ex, ex.Message);
			}
			
			if (settings == null)
			{
				LogThis(DateTime.Now.ToString("s") + " No settings loaded - using default", settings.LogFileName);
				settings = new CSettings();
			}
			
		}

		/*private void checkChat()
		{
			if ((DateTime.Now - this.lastCalled).TotalSeconds > 60)
			{
				SendChat();
				this.lastCalled = DateTime.Now;
			}
		}

		public void SendChat()
		{
			UnturnedChat.Say("Hello! FixedUpdate was called " + callCount.ToString() + " times as of " + DateTime.Now.ToUniversalTime());
		}

		public void FixedUpdate()
		{
			callCount++;

			if (this.State == PluginState.Loaded)
				this.checkChat();
		}*/



		void FixedUpdate()
		{
			if ((DateTime.Now - lastCalled).Seconds >= 1) //Check once per second.
			{
				if (DateTime.Now.CompareTo(settings.startTime) >= 0 && DateTime.Now.CompareTo(settings.endTime) <= 0 && status.isActive == false)
				{
					status.isActive = true;
					UnturnedChat.Say("MATCH BEGINS");
				}

				if (status.isActive == true && DateTime.Now.CompareTo(settings.endTime) >= 0)
				{
					status.isActive = false;
					UnturnedChat.Say("MATCH FINISHED");
					string result = "DRAW";
					if (status.teamAScore > status.teamBScore)
						result = "TEAM A WON";
					if (status.teamAScore < status.teamBScore)
						result = "TEAM B WON";
					UnturnedChat.Say("result: " + result);
				}

				lastCalled = DateTime.Now;
			}
		}
		
		protected void LogThis(String message, string filename)
		{
			try
			{
				StreamWriter file = new StreamWriter(filename, true);
				file.WriteLine(message);
				file.Flush();
				file.Close();
			}
			catch (Exception ex)
			{
				Logger.LogException(ex, ex.Message);
			}
		}
	}
}
