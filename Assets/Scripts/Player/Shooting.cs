using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour {
	public Camera playerCamera;
	private float shootTime = 10f;
  private const float SHOT_DELAY = 0.5f;
	private RaycastHit hit;
	private Ray ray;
	private Vector3 endpoint;
	private float distance;
    private bool canshoot = true;
	[SerializeField] private GameObject tipOfGun;
	[SerializeField] private GameObject armPivot;
	[SerializeField] private AudioSource audioSource;
	[Header("Sounds")]
	[SerializeField] private AudioClip revolverSound;
	private Animator armPivotAnimator;
	private PhotonView photonView;
	LayerMask ignoreRayCastLayer;
	Character player;

	void Start () {
		playerCamera = gameObject.GetComponent<PlayerController>().playerCamera;
		endpoint = new Vector3(0,0,0);
		distance = 0;
		photonView = gameObject.GetComponent<PhotonView>();
		armPivotAnimator = armPivot.GetComponent<Animator>();
		player = gameObject.GetComponent<Character> ();

		// all layers except 2nd which is Ignore Raycast
		ignoreRayCastLayer = ~(1 << 2);
	}

	void Update () {
		if (!photonView.isMine) { return; }
		if (Input.GetButtonDown ("Fire1") && canshoot == true) {
            //Get Point where bullet will hit
            StartCoroutine(delayedShooting());
            armPivotAnimator.SetTrigger("shooting");
			ray = new Ray(playerCamera.transform.position,playerCamera.transform.forward*100);
			if (Physics.Raycast(ray ,out hit, Mathf.Infinity, ignoreRayCastLayer)) {
				endpoint = ray.GetPoint(hit.distance);
			} else {
			endpoint = ray.GetPoint(1000);
			}

			gameObject.GetComponent<PhotonView>().RPC("shoot",PhotonTargets.All, tipOfGun.transform.position,endpoint, player.getUserId());
		}
	}
	[PunRPC]
	private void shoot(Vector3 start, Vector3 end, int userId) {
		audioSource.PlayOneShot(revolverSound);
		if(!PhotonNetwork.isMasterClient) { return; }
		//create the bullet at tip of gun
		PhotonNetwork.Instantiate ("Bullet", start ,Quaternion.LookRotation(Vector3.Normalize(end-start)), 0, new object[] {userId, Vector3.Normalize(end-start)*300, photonView.viewID});
		//shot.GetComponent<Rigidbody>().velocity = Vector3.Normalize(end-start)*300;
		//shot.GetComponent<Bullet> ().setUserId (userId);
	}

    private IEnumerator delayedShooting(){
        canshoot = false;
        yield return new WaitForSeconds(SHOT_DELAY);
        canshoot = true;
    }
}
