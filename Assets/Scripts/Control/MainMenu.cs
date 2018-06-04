using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour {
	[Header("Buttons")]
	[SerializeField] Button buttonFindGame;
	[SerializeField] Button buttonOptions;
	[SerializeField] Button buttonOptionsSave;
	[SerializeField] Button buttonOptionsBack;
	[Header("Inputs")]
	[SerializeField] InputField inputUsername;
	[Header("Sections")]
	[SerializeField] GameObject optionsSection;
	[SerializeField] GameObject mainMenuSection;
	[Header("Other Elements")]
	[SerializeField] GameObject loadingSpinner;
	[SerializeField] GameObject mainPanel;
	void Start() {
		buttonFindGame.onClick.AddListener(findGame);
		buttonOptions.onClick.AddListener(goToOptions);
		buttonOptionsSave.onClick.AddListener(optionsSave);
		buttonOptionsBack.onClick.AddListener(optionsBack);
	}
	private void findGame() {
		loadingSpinner.SetActive(true);
		mainPanel.SetActive(false);
		mainMenuSection.SetActive(false);
		gameObject.GetComponent<NetworkManager>().JoinServer();
	}
	private void goToOptions() {
		optionsSection.SetActive(true);
		mainMenuSection.SetActive(false);
		inputUsername.text = Global.username;
	}
	private void optionsSave() {
		//TODO save data to json file
		string username = inputUsername.text;
		Global.username = username;
		Global.gameSettings.username = username;
		gameObject.GetComponent<DataEditor>().SaveGameData();
		mainMenuSection.SetActive(true);
		optionsSection.SetActive(false);
	}
	private void optionsBack() {
		 mainMenuSection.SetActive(true);
		 optionsSection.SetActive(false);
	}
}
