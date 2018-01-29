using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	Vector3 newOrigin(Vector3 origin, float oldRadius, float newRadius){
		if (newRadius > oldRadius) { return; }
		float tempRadius = Random.Range (0f, (oldRadius - newRadius));
		float angle = Random.Range (0f, 2f * Mathf.PI);
		origin.x = Mathf.Sqrt(tempRadius) * Mathf.Cos (angle);
		origin.z = Mathf.Sqrt(tempRadius) * Mathf.Sin (angle);
		return (origin);
	}
}
