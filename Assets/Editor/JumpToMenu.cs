using UnityEditor;
using UnityEngine;
using System.Collections;

[InitializeOnLoad]
public static class JumpToMenu {
  // click command-0 to go to the prelaunch scene and then play

  [MenuItem("Edit/Play-Unplay, But From Prelaunch Scene %0")]
  public static void PlayFromPrelaunchScene() {
     if (EditorApplication.isPlaying == true) {
         EditorApplication.isPlaying = false;
         return;
      }
     EditorApplication.SaveCurrentSceneIfUserWantsTo();
     EditorApplication.OpenScene("Assets/Scenes/MenuScene.unity");
     EditorApplication.isPlaying = true;
  }
}
