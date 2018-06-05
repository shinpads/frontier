using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour {
	[Header("Buttons")]
	[SerializeField] Button buttonFindGame;
	[SerializeField] Button buttonOptions;
	[SerializeField] Button buttonExit;
	[Header("Sections")]
	[SerializeField] GameObject optionsSection;
	[SerializeField] GameObject mainMenuSection;
	[Header("Other Elements")]
	[SerializeField] GameObject loadingSpinner;
	[SerializeField] GameObject mainPanel;
	void Start() {
		buttonFindGame.onClick.AddListener(findGame);
		buttonOptions.onClick.AddListener(goToOptions);
		buttonExit.onClick.AddListener(exitGame);
	}
	private void findGame() {
		loadingSpinner.SetActive(true);
		mainPanel.SetActive(false);
		mainMenuSection.SetActive(false);
		gameObject.GetComponent<NetworkManager>().JoinServer();
	}
	private void goToOptions() {
		optionsSection.GetComponent<OptionsController>().loadOptions();
		mainMenuSection.SetActive(false);
	}
	private void exitGame() {
		// only works in builds
		Application.Quit();
	}
}
