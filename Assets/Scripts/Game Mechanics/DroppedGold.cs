using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedGold : MonoBehaviour {
	private int teamId;
	private int goldCount;
	private const float LIBERATED_GOLD_LIFESPAN = 30f;
	private GameController gameController;

	void Start () {
		gameController = GameObject.FindWithTag("Control").GetComponent<GameController>();
		object[] data = GetComponent<PhotonView>().instantiationData;
		teamId = (int)data [0];
		goldCount = (int)data [1];
		if (teamId == gameController.getThisTeam ()) {
			gameObject.GetComponent<ParticleSystem> ().Play ();
		}
		StartCoroutine (goldTimeOut ());
	}

	public int getGoldCount() { return goldCount; }
	public int getTeamId() { return teamId; }

	public int pickUpGold(int playerGoldCarry, int playerGoldCapacity) {
		int amount = Mathf.Min(playerGoldCapacity - playerGoldCarry, goldCount);
		if (amount < goldCount) {
			goldCount -= amount;
			return amount;
		} else {
			gameController.sendDestroyGold (gameObject);
			return goldCount;
		}
	}

	public void returnGold() {
		gameController.sendSetCartGold (teamId, goldCount);
		gameController.sendDestroyGold (gameObject);
	}

	private IEnumerator goldTimeOut () {
		yield return new WaitForSeconds(LIBERATED_GOLD_LIFESPAN);
		returnGold ();
	}
}
