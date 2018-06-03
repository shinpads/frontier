using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour {
	[Header("Buttons")]
	[SerializeField] Button buttonFindGame;

	void Start() {
		buttonFindGame.onClick.AddListener(findGame);
	}
	public void findGame() {
		gameObject.GetComponent<NetworkManager>().JoinServer();
	}
}
