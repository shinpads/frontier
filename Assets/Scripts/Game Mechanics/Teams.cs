using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teams : MonoBehaviour {
	private const int PLAYERS_PER_TEAM = 5;
	private const int STARTING_GOLD = 500;
	private int teamId;
	private int playerCount;
	private int goldCount;

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

	public void addPlayer() {
		if (playerCount < PLAYERS_PER_TEAM) {
			playerCount++;
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

	public void setTeamId(int id){
		teamId = id;
	}
}
