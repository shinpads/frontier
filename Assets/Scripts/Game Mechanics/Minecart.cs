using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minecart : MonoBehaviour {
	private const int STARTING_GOLD = 500;
	private int goldCount;
	[SerializeField] private int teamid;
	private float cartSpeed;

	void Start () {
		goldCount = STARTING_GOLD;
	}

	void FixedUpdate() {
		cartSpeed = (float)goldCount / STARTING_GOLD * Time.deltaTime;
		gameObject.transform.Translate (Vector3.forward * cartSpeed);
	}

	public int getTeamId(){
		return teamid;
	}

	public int getGold(){
		return goldCount;
	}
		
	public void setCartGold(int gold) {
		goldCount += gold;
	}
		
	public int loseGold(int playerCarry){
		int lostGold;
		if (playerCarry > goldCount) {
			lostGold = goldCount;
		} 
		else {
			lostGold = playerCarry;
		}
		setCartGold (-lostGold);
		return lostGold;
	}
}
