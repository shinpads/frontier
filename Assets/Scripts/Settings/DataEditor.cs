using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class DataEditor : MonoBehaviour {
	private string gameDataProjectFilePath = "/config.json";
	GameSettings gameSettings;
	void Start() {
		LoadGameData();
	}

	private void LoadGameData() {
			string filePath = Application.dataPath + gameDataProjectFilePath;

			if (File.Exists (filePath)) {
					string dataAsJson = File.ReadAllText (filePath);
					gameSettings = JsonUtility.FromJson<GameSettings> (dataAsJson);
			} else {
				gameSettings = new GameSettings();
				File.WriteAllText (filePath,  JsonUtility.ToJson (gameSettings));
			}
			Global.gameSettings = gameSettings;
			Global.username = gameSettings.username;
	}

	public void SaveGameData() {
			string dataAsJson = JsonUtility.ToJson (Global.gameSettings);

			string filePath = Application.dataPath + gameDataProjectFilePath;
			File.WriteAllText (filePath, dataAsJson);

	}
}
