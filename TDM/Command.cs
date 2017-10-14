using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDM
{
	public class Command : IRocketCommand
	{
		public string Name
		{
			get { return "tdm"; }
		}

		public string Help
		{
			get { return ("Team Deathmatch status"); }
		}

		public AllowedCaller AllowedCaller
		{
			get { return AllowedCaller.Player; }
		}

		public string Syntax
		{
			get { return "Usage: /tdm [A | B]"; }
		}

		public List<string> Aliases
		{
			get { return new List<string>() { }; }
		}

		public List<string> Permissions
		{
			get { return new List<string>() { "TDM.permission" }; }

		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			if (TDM.instance == null)
			{
				UnturnedChat.Say("Something's wrong with TDM plugin!", Color.red);
				return;
			}

			if (command == null || command.Length == 0)
			{
				UnturnedChat.Say(caller, "Team Deathmatch status: " + (TDM.instance.status.isActive ? "ACTIVE" : "INACTIVE"));
				UnturnedChat.Say(caller, "SCORE: " + TDM.instance.status.teamAScore.ToString() + " : " + TDM.instance.status.teamBScore.ToString(), Color.red);
				UnturnedChat.Say(caller, "START TIME: " + TimeSpanToHumanReadableString((TDM.instance.Configuration.Instance.startTime - DateTime.Now), true, true));
				UnturnedChat.Say(caller, "END TIME: " + TimeSpanToHumanReadableString((TDM.instance.Configuration.Instance.endTime - DateTime.Now), true, true));
			}
			else if (command.Length == 1 && (command.ElementAt(0) == "a" || command.ElementAt(0) == "A"))
			{
				UnturnedChat.Say(caller, "Team A score: " + TDM.instance.status.teamAScore.ToString(), Color.red);
				string playerString = "";
				if (TDM.instance.playerList != null && TDM.instance.Configuration.Instance != null)
					foreach (PlayerListItem i in TDM.instance.playerList)
					{
						if (i.teamSteamID == TDM.instance.Configuration.Instance.TeamASteamId)
							playerString = playerString + i.characterName + ", ";
					}
				UnturnedChat.Say(caller, "Team A players: " + playerString);
			}
			else if (command.Length == 1 && (command.ElementAt(0) == "b" || command.ElementAt(0) == "B"))
			{
				UnturnedChat.Say(caller, "Team B score: " + TDM.instance.status.teamBScore.ToString(), Color.red);
				string playerString = "";
				if (TDM.instance.playerList != null && TDM.instance.Configuration.Instance != null)
					foreach (PlayerListItem i in TDM.instance.playerList)
					{
						if (i.teamSteamID == TDM.instance.Configuration.Instance.TeamBSteamId)
							playerString = playerString + i.characterName + ", ";
					}
				UnturnedChat.Say(caller, "Team B players: " + playerString);
			}
			else
			{
				UnturnedChat.Say(caller, "Unknown parameter");
			}
		}

		string TimeSpanToHumanReadableString(TimeSpan span, bool inAgo = false, bool seconds = false)
		{
			string res = "";
			if (Math.Abs(span.TotalDays) > 3)
			{
				if (!inAgo)
					return "more than 3 days";
				else
					return (span.TotalDays > 0 ? "in more than 3 days" : "more than 3 days ago");
			}
			else
			{
				res = (span.Days == 0 ? "" : (Math.Abs(span.Days).ToString() + " days, "))
				+ Math.Abs(span.Hours).ToString() + " hours, "
				+ Math.Abs(span.Minutes).ToString() + " minutes"
				+ (seconds ? (", " + Math.Abs(span.Seconds).ToString() + " seconds") : "");

				if (inAgo)
				{
					if (span.TotalSeconds >= 0)
						res = "in " + res;
					else
						res = res + " ago";
				}
				else
				{
					if (span.TotalSeconds < 0)
						res = "-" + res;
				}
			}
			return res;
		}
	}
}
