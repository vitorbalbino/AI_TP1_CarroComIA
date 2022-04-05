using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour {
    public void OnReset() {
        SceneManager.LoadScene(0, LoadSceneMode.Single);

        Debug.Log("Scene was reloaded");
    }

    public void OnQuit() {

        #if UNITY_EDITOR

            Debug.Log("Game quit");
            UnityEditor.EditorApplication.isPlaying = false;

        #elif UNITY_WEBPLAYER
                 Application.OpenURL(webplayerQuitURL);
        #else
                 Application.Quit();
        #endif
    }
}
