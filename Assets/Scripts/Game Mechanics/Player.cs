using System.Collections;
using System.Collections.Generic;

public class Player {
	private int userId;
	private string username;
	private int classType;
	private Teams team;
	private Stats statLine;

	public class Stats {
		public int kills;
		public int deaths;
		public int assists;
		public int goldStolen;
	}

	public Player (int userId, string username, Teams userTeam) {
		this.userId = userId;
		this.username = username;
		team = userTeam;
		statLine = new Stats ();
		statLine.kills = statLine.deaths = statLine.assists = statLine.goldStolen = 0;
		classType = -1;
	}

	public void setUsername (string username) {
		this.username = username;
	}

	public string getUsername () {
		return this.username;
	}

	public void setClassType (int classType) {
		if (this.classType >= 0) {
			team.setRollsFilled (this.classType, false);
		}
		if (team.getRollState(classType)) { return; }
		this.classType = classType;
		team.setRollsFilled (this.classType, true); 
	}

	public int getClassType () {
		return this.classType;
	}

	public int getUserId () {
		return this.userId;
	}

	public void addDeath() {
		statLine.deaths++;
	}

	public void addKill() {
		statLine.kills++;
	}

	public void addAssist() {
		statLine.assists++;
	}

	public void addGoldStolen(int gold) {
		statLine.goldStolen += gold;
	}

	public Teams getPlayerTeam() { return team; }

	public Stats getStatLine() { return statLine; }
}
