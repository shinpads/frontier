using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour {
	public GameObject[] classMeshes;
	public GameObject[] containers;
	private int classType;

	public void setClass(int refrence) {
		PhotonView photonView = gameObject.GetComponent<PhotonView>();
		classType = refrence;
		photonView.RPC("setMeshEnabled", PhotonTargets.AllBuffered, classType);
	}

	public GameObject getContainer () {
		return containers[classType];
	}

	[PunRPC]
	private void setMeshEnabled(int classType) {
		PhotonView photonView = gameObject.GetComponent<PhotonView>();
		for (int i = 0; i < classMeshes.Length; i++) {
			classMeshes[i].SetActive(false);
			containers[i].SetActive(false);
		}
		classMeshes[classType].SetActive(true);
		containers[classType].SetActive(true);
		this.classType = classType;
		gameObject.GetComponent<Shooting>().setContainer();
		if (photonView.isMine) {
			gameObject.GetComponent<PlayerController>().setArmPivot(containers[classType]);
			SkinnedMeshRenderer[] meshes = classMeshes[classType].GetComponentsInChildren<SkinnedMeshRenderer>();//.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			foreach (SkinnedMeshRenderer renderer in meshes) {
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			}
		}
	}
}
