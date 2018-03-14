using System.Collections;
using System.Collections.Generic;

public class Player {
	private int userId;
	private string username;
	private int classType;
	private Teams team;

	public Player (int userId, string username, Teams userTeam) {
		this.userId = userId;
		this.username = username;
		team = userTeam;
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
}
