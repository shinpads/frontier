using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	Rigidbody rigidbod;
	[SerializeField] string createOnTimeOut;
	[SerializeField] float timeOutTime;
	[SerializeField] float destroyTime;
	[SerializeField] bool freezeOnGround;
	[HideInInspector] public int userId;
	PhotonView photonView;
	bool collided = false;
	void Start() {
		photonView = gameObject.GetComponent<PhotonView> ();
		object[] data = GetComponent<PhotonView>().instantiationData;
		userId = (int)data[0];
		rigidbod = gameObject.GetComponent<Rigidbody>();
		if (photonView.isMine && timeOutTime > 0) {
			StartCoroutine(timeOut ());
		}
	}

	void createObject() {
		gameObject.GetComponent<Renderer>().enabled = false;
		rigidbod.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		PhotonNetwork.Instantiate(createOnTimeOut, gameObject.transform.position, Quaternion.identity, 0, new object[] {userId});
		if (destroyTime > 0) { StartCoroutine(destroyObject()); }
	}


	private IEnumerator destroyObject() {
		yield return new WaitForSeconds(destroyTime);
		PhotonNetwork.Destroy(gameObject);
	}

	private IEnumerator timeOut() {
		yield return new WaitForSeconds (timeOutTime);
		createObject();
	}
}
