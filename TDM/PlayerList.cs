using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.Unturned.Player;

namespace TDM
{
	public class PlayerListItem
	{
		public string characterName;
		public UInt64 steamID;
		public UInt64 teamSteamID;
		public int score;

		public PlayerListItem( string name, UInt64 ID, UInt64 teamID)
		{
			characterName = name;
			steamID = ID;
			teamSteamID = teamID;
		}
	}

//	class PlayerList
//	{
//	}
}
