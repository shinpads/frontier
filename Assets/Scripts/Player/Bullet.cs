using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private const int LIFE_SPAN = 3;
	private Vector3 currentPosition, lastPosition;
	private float positionDifference;
	private RaycastHit hit;
	private Ray ray;
	private Vector3 velocity;
	private int userId;
	private PhotonView userPlayer;
	private int healScore;
	private int damage;
	private Vector3 startSpot;
	private float dropOff, dropOffStop;
	[SerializeField] GameObject dirtImpactParticles;
	private int maxDamage;
	GameController gameController;
	void Start () {
		if(!PhotonNetwork.isMasterClient) { enabled = false; }
		gameController = GameObject.FindWithTag("Control").GetComponent<GameController>();
		object[] data = GetComponent<PhotonView>().instantiationData;
		setUserId ((int)data[0]);
		currentPosition = gameObject.transform.position;
		lastPosition = gameObject.transform.position;
		userPlayer = PhotonView.Find ((int)data [2]);
		velocity = (Vector3)data[1];
		dropOff = (float)data [4];
		dropOffStop = (float)data [5];
		maxDamage = (int)data[3];
		damage = maxDamage;
		startSpot = gameObject.transform.position;
		StartCoroutine(setTimeOutDestroy());
	}

	public void setUserId(int id) {
		userId = id;
	}

	void Update() {
		velocity.y -= 9.81f * Time.deltaTime;
		gameObject.transform.position = gameObject.transform.position + (velocity * Time.deltaTime);
		gameObject.transform.rotation = Quaternion.LookRotation(velocity.normalized);
		currentPosition = gameObject.transform.position;
		positionDifference = (currentPosition - lastPosition).magnitude;
		// check if there was a collision in the last 0.1 units
		if (positionDifference > 0.1f) {
			ray = new Ray(lastPosition, velocity.normalized);
			if (Physics.Raycast(ray, out hit, positionDifference)) {
				if (hit.collider.gameObject.tag == "Player") {
					damage = Mathf.RoundToInt (((maxDamage-1)*(100 - (Mathf.Clamp(Vector3.Distance (startSpot, hit.point),dropOff, dropOffStop) - dropOff) * (100/(dropOffStop - dropOff)))/100) + 1);
					hit.collider.gameObject.GetComponent<PhotonView> ().RPC("setHealth", PhotonTargets.All, -damage, userId);
				} else if (hit.collider.gameObject.tag == "TargetCircle") {
					gameController.sendHitMarked (userPlayer.GetComponent<Character> ().getUserId ());
					healScore = hit.collider.gameObject.GetComponentInParent<Target> ().hitTarget (hit.collider.gameObject);
					if (healScore > 0) {
						userPlayer.GetComponent<PhotonView> ().RPC ("setHealth", PhotonTargets.All, healScore, -1);
					}
				} else {
					PhotonNetwork.Instantiate ("WFX_BImpact Sand", ray.GetPoint(hit.distance), Quaternion.LookRotation(hit.normal), 0);
				}
				PhotonNetwork.Destroy(gameObject);
			}
			lastPosition = currentPosition;
		}
	}
	private IEnumerator setTimeOutDestroy () {
		yield return new WaitForSeconds(LIFE_SPAN);
		PhotonNetwork.Destroy(gameObject);
	}
}
