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
using SDG.Unturned;

/// <summary>
/// Basic Team Deathmatch plugin for Rocket https://github.com/RocketMod/Rocket
/// </summary>

namespace TDM
{
	public class TDM : RocketPlugin <Configuration>
	{
		public CStatus status;      //initialized in LoadStatus
		public DateTime lastCalled;

		public static TDM instance;

		public Dictionary<CSteamID, PlayerListItem> playerList;


		protected override void Load()
		{
			instance = this;
			lastCalled = DateTime.Now;
			playerList = new Dictionary<CSteamID, PlayerListItem>();

			LoadStatus();
			LogThis(DateTime.Now.ToString("s") + " TDM Loaded", instance.Configuration.Instance.logFileName);
			UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;
			U.Events.OnPlayerConnected += UnturnedEvents_OnPlayerConnected;
			U.Events.OnPlayerDisconnected += UnturnedEvents_OnPlayerDisconnected;


		}


		protected override void Unload()
		{
			LogThis(DateTime.Now.ToString("s") + " TDM Unloaded", instance.Configuration.Instance.logFileName);
			UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
			U.Events.OnPlayerConnected -= UnturnedEvents_OnPlayerConnected;
			U.Events.OnPlayerDisconnected -= UnturnedEvents_OnPlayerDisconnected;

			instance = null;
			status = null;
			playerList = null;
		}


		private void UnturnedPlayerEvents_OnPlayerDeath(UnturnedPlayer player, SDG.Unturned.EDeathCause cause, SDG.Unturned.ELimb limb, CSteamID murderer)
		{
			try
			{
				LogThis(DateTime.Now.ToString("s") + ";" + player.CSteamID.ToString() + ";" + player.SteamGroupID.ToString() + ";" + cause.ToString() + ";" + murderer.ToString() + ";", instance.Configuration.Instance.fragLogFileName);

				if (!status.isActive)
					return;

				if (player.SteamGroupID.m_SteamID == instance.Configuration.Instance.teamASteamId && status.isActive)
				{
					status.teamBScore += 1;
					SaveStatus();
					UnturnedChat.Say("SCORE: " + status.teamAScore.ToString() + " : " + status.teamBScore.ToString() + " - Team B scored a point!", TDM.instance.Configuration.Instance.scoreColor);
				}
				if (player.SteamGroupID.m_SteamID == instance.Configuration.Instance.teamBSteamId && status.isActive)
				{
					status.teamAScore += 1;
					SaveStatus();
					UnturnedChat.Say("SCORE: " + status.teamAScore.ToString() + " : " + status.teamBScore.ToString() + " - Team A scored a point!", TDM.instance.Configuration.Instance.scoreColor);
				}
			}
			catch (Exception ex)
			{
				Rocket.Core.Logging.Logger.LogException(ex, ex.Message);
			}
		}


		private void UnturnedEvents_OnPlayerConnected(UnturnedPlayer player)
		{
			LogThis(DateTime.Now.ToString("s") + ";" + player.CSteamID.ToString() + ";" + player.SteamGroupID.ToString() + ";" + "Connected" + ";" + ";" + player.CharacterName + ";" + player.IP + ";", instance.Configuration.Instance.fragLogFileName);

			UpdatePlayerList();

			if (player.SteamGroupID.m_SteamID == instance.Configuration.Instance.teamASteamId)
			{
				UnturnedChat.Say(player.CharacterName + " has joined team A", TDM.instance.Configuration.Instance.messageColor);
			}
			else if (player.SteamGroupID.m_SteamID == instance.Configuration.Instance.teamBSteamId)
			{
				UnturnedChat.Say(player.CharacterName + " has joined team B", TDM.instance.Configuration.Instance.messageColor);
			}
			else
			{
				UnturnedChat.Say(player.CharacterName + " haven't set their team setting correctly."
								+ (instance.Configuration.Instance.kickPlayerWithInvalidTeam ? (" Kicking in " + instance.Configuration.Instance.kickDelaySeconds.ToString()) + " seconds" : ""), TDM.instance.Configuration.Instance.messageColor);
				if (instance.Configuration.Instance.kickPlayerWithInvalidTeam)
				{
					StartCoroutine(KickPlayer(player, instance.Configuration.Instance.kickDelaySeconds, "   To play on this server please select one of these groups as primary in your Unturned settings (Survivors -> Group -> Group):\n"
						+ instance.Configuration.Instance.teamASteamUri + "    " + instance.Configuration.Instance.teamBSteamUri));
				}
			}
		}


		private void UnturnedEvents_OnPlayerDisconnected(UnturnedPlayer player)
		{
			LogThis(DateTime.Now.ToString("s") + ";" + player.CSteamID.ToString() + ";" + player.SteamGroupID.ToString() + ";" + "Disconnected" + ";" + ";", instance.Configuration.Instance.fragLogFileName);
			UpdatePlayerList();
		}


		public void UpdatePlayerList()
		{
			Dictionary<CSteamID, PlayerListItem> newPlayerList = new Dictionary<CSteamID, PlayerListItem>();
			foreach (SteamPlayer p in Provider.clients)
			{
				PlayerListItem item = null;

				if (playerList.TryGetValue(p.playerID.steamID, out item))
				{
					newPlayerList.Add(p.playerID.steamID, item);
				}
				else 
				{
					newPlayerList.Add(p.playerID.steamID, new PlayerListItem(p.playerID.characterName, p.playerID.group.m_SteamID));
				}
			}
			playerList = newPlayerList;
		}


		public void SaveStatus()
		{
			XmlSerializer xs = new XmlSerializer(typeof(CStatus));
			TextWriter tw = new StreamWriter(instance.Configuration.Instance.statusFileName);
			xs.Serialize(tw, status);
			tw.Flush();
			tw.Close();
		}


		public void LoadStatus()
		{
			XmlSerializer xs = new XmlSerializer(typeof(CStatus));
			try
			{
				StreamReader sr = new StreamReader(instance.Configuration.Instance.statusFileName);
				status = (CStatus)xs.Deserialize(sr);
				sr.Close();
			}
			catch (Exception ex)
			{
				Rocket.Core.Logging.Logger.LogException(ex, ex.Message);
			}

			if (status == null)
			{
				LogThis(DateTime.Now.ToString("s") + " No status loaded - using default", instance.Configuration.Instance.logFileName);
				status = new CStatus();
				SaveStatus();
			}

		}


		void FixedUpdate()
		{
			if (instance == null)
				return;

			if ((DateTime.Now - lastCalled).Seconds >= 1) //Check once per second.
			{
				if (DateTime.Now.CompareTo(instance.Configuration.Instance.startTime) >= 0 && DateTime.Now.CompareTo(instance.Configuration.Instance.endTime) <= 0 && status.isActive == false)
				{
					status.isActive = true;
					UnturnedChat.Say("MATCH BEGINS", TDM.instance.Configuration.Instance.messageColor);
				}

				if (status.isActive == true && DateTime.Now.CompareTo(instance.Configuration.Instance.endTime) >= 0)
				{
					status.isActive = false;
					UnturnedChat.Say("MATCH FINISHED", TDM.instance.Configuration.Instance.messageColor);
					string result = "DRAW";
					if (status.teamAScore > status.teamBScore)
						result = "TEAM A WON";
					if (status.teamAScore < status.teamBScore)
						result = "TEAM B WON";
					UnturnedChat.Say("result: " + result, TDM.instance.Configuration.Instance.scoreColor);
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
