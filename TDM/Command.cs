using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			get { return "Usage: /tdm"; }
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
			UnturnedChat.Say("Team Deathmatch status: " + (TDM.instance.status.isActive ? "ACTIVE" : "INACTIVE"));
			UnturnedChat.Say("SCORE: " + TDM.instance.status.teamAScore.ToString() + " : " + TDM.instance.status.teamBScore.ToString());
			UnturnedChat.Say("START TIME: " + TimeSpanToHumanReadableString((TDM.instance.settings.startTime - DateTime.Now), true, true));
			UnturnedChat.Say("END TIME: " + TimeSpanToHumanReadableString((TDM.instance.settings.endTime - DateTime.Now), true, true));
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
				} else {
					if (span.TotalSeconds < 0)
						res = "-" + res;
				}
			}
			return res;
		}
	}
}
