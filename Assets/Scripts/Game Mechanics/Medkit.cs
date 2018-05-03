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
		canHeal = true;
		object[] data = GetComponent<PhotonView>().instantiationData;
		teamId = (int)data[1];
	}

	void OnTriggerEnter(Collider col) {
		if (!PhotonNetwork.isMasterClient || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() != teamId) { return; }
		insidePlayers.Add(col.gameObject.GetComponent<Character>());
	}

	void OnTriggerStay(Collider col) {
		if (!PhotonNetwork.isMasterClient || !canHeal || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() != teamId) { return; }
		foreach (Character player in insidePlayers) {
			int appliedHealing = player.getMaxHealth () - player.getCurrentHealth ();
			if (appliedHealing == 0) { continue; }
			if (appliedHealing > healthPerSecond) {
				appliedHealing = healthPerSecond;
			}
			if (appliedHealing > health) {
				appliedHealing = health;
			}
			Debug.Log("sending rpc");
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
