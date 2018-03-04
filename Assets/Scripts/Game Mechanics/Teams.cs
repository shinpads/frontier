using System.Collections;
using System.Collections.Generic;
public class Teams {
	private const int PLAYERS_PER_TEAM = 5;
	private const int STARTING_GOLD = 500;
	private int teamId;
	private int playerCount;
	private int goldCount;
	public int[] playerIds = new int[5];
	// [Tank, Scout, Thief, Other, Assualt]
	private bool[] rolesFilled = { false, false, false, false, false };

	public Teams (int id) {
		teamId = id;
		playerCount = 0;
		goldCount = STARTING_GOLD;
	}

	public int getId () {
		return teamId;
	}

	public int getPlayerCount() {
		return playerCount;
	}

	public bool addPlayer(int userId) {
		if (playerCount < PLAYERS_PER_TEAM) {
			playerIds [playerCount] = userId;
			playerCount++;
			return true;
		}
		else {
			return false;
		}
	}

	public void removePlayer() {
		if (playerCount > 0) {
			playerCount--;
		}
	}

	public void setGold(int gold) {
		if (!(goldCount == 0 && gold < 0)) {
			goldCount += gold;
		}
		if (goldCount < 0) {
			goldCount = 0;
		}
	}

	public int getGold(){
		return goldCount;
	}
}
