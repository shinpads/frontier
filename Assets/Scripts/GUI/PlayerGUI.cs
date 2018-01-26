using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGUI : MonoBehaviour {
	[SerializeField] Texture crosshair;
	void OnGUI() {
		GUI.Label(new Rect(Screen.width/2 - 16f,Screen.height/2 - 16f, 32f,32f),crosshair);
	}

}
