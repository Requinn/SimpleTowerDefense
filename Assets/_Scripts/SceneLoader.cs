using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(int sceneIndex) {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    private IEnumerator LoadAsync(int index) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = false;

        //spinlock while loading
        while(asyncLoad.progress < 0.89f) {
             yield return 0f;
        }

        asyncLoad.allowSceneActivation = true;
    }
}
