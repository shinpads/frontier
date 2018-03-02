using UnityEngine;
using System.Collections;
public class TransformInterpolate : MonoBehaviour {
	private Vector3 realPos = Vector3.zero;
	private Quaternion realRot = Quaternion.identity; 
	//Send & Read over Network
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		if (stream.isWriting) {
			//send your position over network
			Vector3 _pos = transform.position;
			Quaternion _rot = transform.rotation;
			stream.Serialize (ref _pos);
			stream.Serialize (ref _rot);

		} else if (stream.isReading) {
			stream.Serialize (ref realPos);
			stream.Serialize (ref realRot);

		}
	}

	void Update(){

		if (GetComponent<NetworkView> ().isMine)
			return;
		transform.rotation = Quaternion.Lerp (transform.rotation, realRot, Time.deltaTime * 15f);
		transform.position = Vector3.Lerp (transform.position,realPos, Time.deltaTime*15f);
	}


}
