using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour {
	public GameObject[] classMeshes;
	private int classType;

	public void setClass(int refrence) {
		PhotonView photonView = gameObject.GetComponent<PhotonView>();
		for (int i = 0; i < classMeshes.Length; i++) {
		photonView.RPC("setMeshEnabled", PhotonTargets.All, i, false);
		}
		classType = refrence;
		photonView.RPC("setMeshEnabled", PhotonTargets.All, refrence, true);
	}

	[PunRPC]
	private void setMeshEnabled(int i, bool enabled) {
		classMeshes[i].SetActive(false);
	}
}
