using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Steamworks;
using UnityEngine;

using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;


/// <summary>
/// Basic Team Deathmatch plugin for Rocket https://github.com/RocketMod/Rocket
/// </summary>

namespace TDM
{
	public class TDM : RocketPlugin
	{
		public CSettings settings;  //initialized in LoadSettings
		public CStatus status;      //initialized in LoadStatus
		public DateTime lastCalled;

		public static TDM instance;

		public List<PlayerListItem> playerList;


		protected override void Load()
		{
			instance = this;
			lastCalled = DateTime.Now;

			playerList = new List<PlayerListItem>();

			/*XmlSerializer xs = new XmlSerializer(typeof(CSettings));
			TextWriter tw = new StreamWriter(@"Plugins\TDMsettings.xml");
			settings = new CSettings();
			xs.Serialize(tw, settings);
			tw.Flush();
			tw.Close();
			settings = null;*/

			LoadSettings();
			LoadStatus();
			LogThis(DateTime.Now.ToString("s") + " TDM Loaded", settings.LogFileName);
			UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;
			U.Events.OnPlayerConnected += UnturnedEvents_OnPlayerConnected;
			U.Events.OnPlayerDisconnected += UnturnedEvents_OnPlayerDisconnected;


		}


		protected override void Unload()
		{
			LogThis(DateTime.Now.ToString("s") + " TDM Unloaded", settings.LogFileName);
			UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
			U.Events.OnPlayerConnected -= UnturnedEvents_OnPlayerConnected;
			U.Events.OnPlayerDisconnected -= UnturnedEvents_OnPlayerDisconnected;

			instance = null;
			settings = null;
			status = null;
			playerList = null;
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
				Rocket.Core.Logging.Logger.LogException(ex, ex.Message);
			}
		}


		private void UnturnedEvents_OnPlayerConnected(UnturnedPlayer player)
		{
			LogThis(DateTime.Now.ToString("s") + ";" + player.CSteamID.ToString() + ";" + player.SteamGroupID.ToString() + ";" + "Connected" + ";" + ";" + player.CharacterName + ";" + player.IP + ";", settings.FragLogFileName);

			playerList.Add(new PlayerListItem(player.CharacterName, player.CSteamID.m_SteamID, player.SteamGroupID.m_SteamID));

			if (player.SteamGroupID.m_SteamID == settings.TeamASteamId)
			{
				UnturnedChat.Say(player.CharacterName + " has joined team A");
			}
			else if (player.SteamGroupID.m_SteamID == settings.TeamBSteamId)
			{
				UnturnedChat.Say(player.CharacterName + " has joined team B");
			}
			else
			{
				UnturnedChat.Say(player.CharacterName + " haven't set their team setting correctly."
								+ (settings.kickPlayerWithInvalidTeam ? (" Kicking in " + settings.kickDelaySeconds.ToString()) + " seconds" : ""));
				if (settings.kickPlayerWithInvalidTeam)
				{
					StartCoroutine(KickPlayer(player, settings.kickDelaySeconds, "   To play on this server please select one of these groups as primary in your Unturned settings (Survivors -> Group -> Group):\n"
						+ settings.TeamASteamUri + "    " + settings.TeamBSteamUri));
				}
			}
		}


		//private static bool hasSpecificId(UInt64 steamID, PlayerListItem item)
		//{
		//	return (item.steamID == steamID);
		//}


		private void UnturnedEvents_OnPlayerDisconnected(UnturnedPlayer player)
		{
			LogThis(DateTime.Now.ToString("s") + ";" + player.CSteamID.ToString() + ";" + player.SteamGroupID.ToString() + ";" + "Disconnected" + ";" + ";", settings.FragLogFileName);
			playerList.RemoveAll(item => item.steamID == player.CSteamID.m_SteamID);
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
			}
			catch (Exception ex)
			{
				Rocket.Core.Logging.Logger.LogException(ex, ex.Message);
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
				Rocket.Core.Logging.Logger.LogException(ex, ex.Message);
			}

			if (settings == null)
			{
				LogThis(DateTime.Now.ToString("s") + " No settings loaded - using default", settings.LogFileName);
				settings = new CSettings();
			}

		}


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
				Rocket.Core.Logging.Logger.LogException(ex, ex.Message);
			}
		}


		public IEnumerator KickPlayer(UnturnedPlayer player, uint delaySeconds, string reason)
		{
			if (player.HasPermission("kick.ignore"))
			{
				yield break;
			}
			else if (delaySeconds <= 0f)
			{
				yield return new WaitForSeconds(1f);
				player.Kick(reason);
			}
			else
			{
				yield return new WaitForSeconds(delaySeconds);
				player.Kick(reason);
			}
		}
	}
}
