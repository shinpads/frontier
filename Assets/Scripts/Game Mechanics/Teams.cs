using System.Collections;
using System.Collections.Generic;

public class Teams {
	private const int PLAYERS_PER_TEAM = 5;
	private int teamId;
	private int playerCount;
	// [Tank, Scout, Thief, Other, Assualt]
	private bool[] rolesFilled = { false, false, false, false, false };
	private Player[] teamStats = { null, null, null, null, null };
	private Minecart teamCart;
	public Dictionary<int, Player> players = new Dictionary<int, Player> ();

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
			players [userId] = new Player(userId, username, this);
			playerCount++;
			return players[userId];
		}
		return null;
	}

	public Player findPlayerByUserId (int userId) {
		return players [userId];
	}
		

	public void setRollsFilled(int classReference, bool state) {
		rolesFilled [classReference] = state;
	}

	public bool getRollState(int classReference) {
		return rolesFilled [classReference];
	}

	public bool teamClassesPicked() {
		foreach (KeyValuePair<int, Player> entry in players) {
			if (entry.Value == null) { continue; }
			if (entry.Value.getClassType() < 0) {
				return false;
			}
		}
		return true;
	}

	public Dictionary<int, Player> getPlayerDict() { return players; }

	public Player[] getTeamStats() {
		foreach (KeyValuePair<int, Player> entry in players) {
			teamStats [entry.Value.getClassType ()] = entry.Value;
		}
		return teamStats;
	}
}
