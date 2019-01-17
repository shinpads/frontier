using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour {
	[SerializeField] GameObject camera;
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

	// Camera movement
	Vector3 cameraMenuPosition;
	Vector3 cameraLoadingPosition;


	void Start() {
		// buttons
		buttonFindGame.onClick.AddListener(findGame);
		buttonOptions.onClick.AddListener(goToOptions);
		buttonExit.onClick.AddListener(exitGame);

		// camera position
		cameraMenuPosition = camera.transform.position;
		cameraLoadingPosition = cameraMenuPosition;
		cameraLoadingPosition.x -= 450f;
	}

	private void findGame() {
		StartCoroutine(moveCamera(cameraMenuPosition, cameraLoadingPosition, 0.35f));
	}

	private void goToOptions() {
		optionsSection.GetComponent<OptionsController>().loadOptions();
		mainMenuSection.SetActive(false);
	}

	private void exitGame() {
		// only works in builds
		Application.Quit();
	}

	private IEnumerator moveCamera(Vector3 from, Vector3 to, float time) {
		float startTime = Time.time;
		float endTime = startTime + time;
		while (Time.time < endTime) {
			camera.transform.position = Vector3.Lerp(from, to, (Time.time - startTime) / time);
			yield return new WaitForEndOfFrame();
		}
		loadingSpinner.SetActive(true);
		mainPanel.SetActive(false);
		mainMenuSection.SetActive(false);
		gameObject.GetComponent<NetworkManager>().JoinServer();
	}
}
