using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	public static int characterHealth;
	// Use this for initialization
	void Start () {
		characterHealth = 100;
	}
	
	// Update is called once per frame
	void Update(){
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			characterHealth--;
		}
	}

}
