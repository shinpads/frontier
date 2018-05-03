using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour {
	private const int fullHealth = 100;
	private const int healthPerSecond = 10;
	private int health, teamId;
	private bool canHeal;
	ArrayList insidePlayers = new ArrayList();

	void Start () {
		health = fullHealth;
		//set players teamId
		teamId = 0;
		canHeal = true;
	}

	void OnTriggerEnter(Collider col) {
		Debug.Log ("cow");
		if (!PhotonNetwork.isMasterClient || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() != teamId) { return; }
		insidePlayers.Add(col.gameObject.GetComponent<Character>());
		Debug.Log ("Added");
	}

	void OnTriggerStay(Collider col) {
		if (!PhotonNetwork.isMasterClient || !canHeal || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() != teamId) { return; }
		foreach (Character player in insidePlayers) {
			Debug.Log ("Loop");
			int appliedHealing = player.getMaxHealth () - player.getCurrentHealth ();
			if (appliedHealing == 0) { continue; }
			if (appliedHealing > healthPerSecond) {
				appliedHealing = healthPerSecond;
			}
			if (appliedHealing > health) {
				appliedHealing = health;
			}
			player.gameObject.GetComponent<PhotonView>().RPC ("setHealth", PhotonTargets.All, appliedHealing, -1);
			health -= appliedHealing;
			if (health <= 0) {
				destroy ();
			}
			StartCoroutine(waitOneSecond());
		}
	}

	void OnTriggerExit(Collider col) {
		if (!PhotonNetwork.isMasterClient || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() != teamId) { return; }
		Debug.Log ("Gone");
		insidePlayers.Remove(col.gameObject.GetComponent<Character>());
	}

	public void destroy() {
		PhotonNetwork.Destroy (gameObject);
	}

	private IEnumerator waitOneSecond() {
		canHeal = false;
		yield return new WaitForSeconds(1);
		canHeal = true;
	}
}
