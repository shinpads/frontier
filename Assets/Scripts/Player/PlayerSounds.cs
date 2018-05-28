using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip headShotSound;
	[SerializeField] AudioClip defaultDamageSound;

	public void playSound(Global.SOUND_TYPE soundType) {
		PhotonView photonView = gameObject.GetComponent<PhotonView>();
		photonView.RPC("playSoundRPC", PhotonTargets.All, soundType);
	}
	[PunRPC]
	public void playSoundRPC(Global.SOUND_TYPE soundType) {
		AudioClip sound;
		switch (soundType) {
			case Global.SOUND_TYPE.HEADSHOT:
				sound = headShotSound;
				break;
			default:
				sound = defaultDamageSound;
				break;
		}
		audioSource.PlayOneShot(sound);
	}
}
