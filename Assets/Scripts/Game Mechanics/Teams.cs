using System.Collections;
using System.Collections.Generic;

public class Teams {
	private const int PLAYERS_PER_TEAM = 5;
	private int teamId;
	private int playerCount;
	// [Tank, Scout, Thief, Other, Assualt]
	private bool[] rolesFilled = { false, false, false, false, false };
	private Minecart teamCart;
	public Player[] players = new Player[PLAYERS_PER_TEAM];
	private Dictionary<int, Stats> teamStats = new Dictionary<int, Stats>();

	public class Stats {
		public int kills;
		public int deaths;
		public int assists;
	}

	public Teams (int id) {
		teamId = id;
		playerCount = 0;
	}

	public int getId () {
		return teamId;
	}

	public int getPlayerCount() {
		return playerCount;
	}

	public Player addPlayer(int userId, string username) {
		if (playerCount < PLAYERS_PER_TEAM) {
			players [playerCount] = new Player(userId, username);
			Stats playerStats = new Stats ();
			playerStats.kills = playerStats.deaths = playerStats.assists = 0;
			teamStats.Add (userId, playerStats);
			playerCount++;
			return players[playerCount-1];
		}
		return null;
	}
		
	public Player findPlayerByUserId (int userId) {
		for (int i = 0; i < playerCount; i++) {
			if (players[i].getUserId() == userId) {
				return players[i];
			}
		}
		return null;
	}

	public void addPlayerDeath(int userId) {
		teamStats [userId].deaths++;
	}

	public void addPlayerKill(int userId) {
		teamStats [userId].kills++;
	}

	public void addPlayerAssist(int userId) {
		teamStats [userId].assists++;
	}

	public Dictionary<int, Stats> getScore() {
		return teamStats;
	}
}
