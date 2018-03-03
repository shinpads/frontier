using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teams : MonoBehaviour {
	private const int PLAYERS_PER_TEAM = 5;
	private const int STARTING_GOLD = 500;
	private int teamId;
	private int playerCount;
	private int goldCount;
	private string[] playerNames = new string[5];
	// [Tank, Scout, Thief, Other, Assualt]
	private bool[] rolesFilled = { false, false, false, false, false };

	public Teams (int id) {
		teamId = id;
	}

	void Start () {
		playerCount = 0;
		goldCount = STARTING_GOLD;
	}

	public int getId() {
		return teamId;
	}

	public int getPlayerCount() {
		return playerCount;
	}

	public bool addPlayer(string username, int characterType) {
		if (playerCount < PLAYERS_PER_TEAM) {
			playerNames [playerCount] = username;
			rolesFilled [characterType] = true;
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
