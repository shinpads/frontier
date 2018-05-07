using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private Vector3 currentPosition, lastPosition;
	private float positionDifference;
	public float lifeTime;
	private RaycastHit hit;
	private Ray ray;
	public Vector3 velocity;
	public int userId;
	public PhotonView userPlayer;
	public int healScore;
	public int damage;
	public Vector3 startSpot;
	public float dropOff, dropOffStop;
	LayerMask ignoreRayCastLayer;
	[SerializeField] GameObject dirtImpactParticles;
	public int maxDamage;
	GameController gameController;
	void Start () {
		// if(!PhotonNetwork.isMasterClient) { enabled = false; return;}
		gameController = GameObject.FindWithTag("Control").GetComponent<GameController>();;
		currentPosition = gameObject.transform.position;
		lastPosition = gameObject.transform.position;
		startSpot = gameObject.transform.position;
		ignoreRayCastLayer = ~((1 << 13) | (1 << 2));
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
			if (Physics.Raycast(ray, out hit, positionDifference, ignoreRayCastLayer, QueryTriggerInteraction.Ignore)) {
				if (PhotonNetwork.isMasterClient) {
					if (hit.collider.gameObject.tag == "Player" && hit.collider.gameObject.GetComponent<Character>().getUserId() != userId) {
						damage = Mathf.RoundToInt (((maxDamage-1)*(100 - (Mathf.Clamp(Vector3.Distance (startSpot, hit.point),dropOff, dropOffStop) - dropOff) * (100/(dropOffStop - dropOff)))/100) + 1);
						hit.collider.gameObject.GetComponent<PhotonView> ().RPC("setHealth", PhotonTargets.All, -damage, userId);
					} else if (hit.collider.gameObject.tag == "TargetCircle") {
						gameController.sendHitMarked (userId);
						healScore = hit.collider.gameObject.GetComponentInParent<Target> ().hitTarget (hit.collider.gameObject);
						if (healScore > 0) {
							userPlayer.GetComponent<PhotonView> ().RPC ("setHealth", PhotonTargets.All, healScore, -1);
						}
					} else {
						PhotonNetwork.Instantiate ("WFX_BImpact Sand", ray.GetPoint(hit.distance), Quaternion.LookRotation(hit.normal), 0);
					}
				}
				GameObject.Destroy(gameObject);
			}
			lastPosition = currentPosition;
		}
	}
	private IEnumerator setTimeOutDestroy () {
		yield return new WaitForSeconds(lifeTime);
		GameObject.Destroy(gameObject);
	}
}
