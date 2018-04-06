using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour {
	public GameObject[] classMeshes;
	private int classType;

	public void setClass(int refrence) {
		PhotonView photonView = gameObject.GetComponent<PhotonView>();
		classType = refrence;
		photonView.RPC("setMeshEnabled", PhotonTargets.All, classType);
	}

	[PunRPC]
	private void setMeshEnabled(int classType) {
		for (int i = 0; i < classMeshes.Length; i++) {
			classMeshes[i].SetActive(false);
		}
		classMeshes[classType].SetActive(true);
	}
}
