# RocketTDM
A plugin for Rocket that provides Team Deathmatch gamemode:
- A team earns a point when a player from another team dies from any cause.
- The score is stored in a separate XML human-readable file — not affected by server restarts or crashes.
- Two teams based on Steam groups, which SteamIDs and URLs are to be provided in the config file.
- Optional autokick for players not belonging to these groups.
- Optional timer for match control: the plugin will keep the score during a specified time interval (provided in the config file) only.
- Death logging (separate configurable CSV-like logfile).
### Commands:
**/tdm** - print match status, score and time from/to match start and end to chat

**/tdm a** - print players and score for Team A

**/tdm b** - print players and score for Team B

### Notes:
- Does not use Translations file, all strings are hardcoded.
- The score has to be reset manually.
