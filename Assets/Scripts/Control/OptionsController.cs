using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionsController : MonoBehaviour {
	[SerializeField] DataEditor dataEditor;
	[SerializeField] GameObject enableOnDone;
	[Header("Buttons")]
	[SerializeField] Button buttonOptionsSave;
	[SerializeField] Button buttonOptionsBack;
	[Header("Inputs")]
	[SerializeField] InputField inputUsername;
	[SerializeField] InputField inputSensitivity;
	[SerializeField] InputField inputAdsSensitivity;
	[SerializeField] InputField inputScopeSensitivity;
	[Header("Sliders")]
	[SerializeField] Slider sliderSensitivity;
	[SerializeField] Slider sliderAdsSensitivity;
	[SerializeField] Slider sliderScopeSensitivity;
	void Start() {
		buttonOptionsSave.onClick.AddListener(optionsSave);
		buttonOptionsBack.onClick.AddListener(optionsBack);
		sliderSensitivity.onValueChanged.AddListener(sensitivitySliderUpdate);
		sliderAdsSensitivity.onValueChanged.AddListener(adsSensitivitySliderUpdate);
		sliderScopeSensitivity.onValueChanged.AddListener(scopeSensitivitySliderUpdate);
		inputSensitivity.onValueChanged.AddListener(sensitivityInputUpdate);
		inputAdsSensitivity.onValueChanged.AddListener(adsSensitivityInputUpdate);
		inputScopeSensitivity.onValueChanged.AddListener(scopeSensitivityInputUpdate);
	}

	public void loadOptions() {
		//populate fields with settings data
		gameObject.SetActive(true);
		inputUsername.text = Global.username;
		inputSensitivity.text = Global.gameSettings.sensitivity.ToString();
		inputAdsSensitivity.text = Global.gameSettings.ads_sensitivity.ToString();
		inputScopeSensitivity.text = Global.gameSettings.scoped_sensitivity.ToString();
		sliderSensitivity.value = Global.gameSettings.sensitivity;
		sliderAdsSensitivity.value = Global.gameSettings.ads_sensitivity;
		sliderScopeSensitivity.value = Global.gameSettings.scoped_sensitivity;
	}
	private void optionsSave() {
		//TODO save data to json file
		string username = inputUsername.text;
		Global.username = username;
		Global.gameSettings.username = username;
		Global.gameSettings.sensitivity = float.Parse(inputSensitivity.text);
		Global.gameSettings.ads_sensitivity = float.Parse(inputAdsSensitivity.text);
		Global.gameSettings.scoped_sensitivity = float.Parse(inputScopeSensitivity.text);
		dataEditor.SaveGameData();
		optionsBack();
	}
	private void optionsBack() {
		 enableOnDone.SetActive(true);
		 gameObject.SetActive(false);
	}
	private void sensitivitySliderUpdate(float value) {
		value = Mathf.Round(value * 100f) / 100f;
		inputSensitivity.text = value.ToString();
	}
	private void sensitivityInputUpdate(string value) {
		sliderSensitivity.value = float.Parse(value);
	}
	private void adsSensitivitySliderUpdate(float value) {
		value = Mathf.Round(value * 100f) / 100f;
		inputAdsSensitivity.text = value.ToString();
	}
	private void adsSensitivityInputUpdate(string value) {
		sliderAdsSensitivity.value = float.Parse(value);
	}
	private void scopeSensitivitySliderUpdate(float value) {
		value = Mathf.Round(value * 100f) / 100f;
		inputScopeSensitivity.text = value.ToString();
	}
	private void scopeSensitivityInputUpdate(string value) {
		sliderScopeSensitivity.value = float.Parse(value);
	}
}
