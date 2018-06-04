using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour {
	[SerializeField] RectTransform progress;
	float speed = -300f;

	void Update() {
		progress.Rotate(0f, 0f, speed * Time.deltaTime);
	}
}
