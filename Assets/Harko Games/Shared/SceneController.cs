using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : SingletonMonoBehaviour<SceneController>
{
    private bool unloading = false;

    public static void ShowCurrentScenes()
    {
        string output = "";
        if (SceneManager.sceneCount > 0)
        {
            for (int n = 0; n < SceneManager.sceneCount; ++n)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                output += scene.name;
                output += scene.isLoaded ? " (Loaded, " : " (Not Loaded, ";
                output += scene.isDirty ? "Dirty, " : "Clean, ";
                output += scene.buildIndex >= 0 ? " in build)\n" : " NOT in build)\n";
            }
        }
        else
        {
            output = "No open Scenes.";
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayDialog("Scene Summary", output, "Ok");
#else
        Debug.Log(output);
#endif
    }

    public IEnumerator LoadNewScene(int newSceneIndex, System.Action callback = null)
    {
        var load = SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Additive);
        while (!load.isDone)
        {
            yield return null;
        }
        Camera.main.gameObject.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newSceneIndex));
        //LevelLoadPostLogic((SceneNames)newSceneIndex);
        //callback?.Invoke();
        if (callback != null)
        {
            callback.Invoke();
        }
        yield return null;
    }

    public IEnumerator UnloadCurrentAndLoadNewScene(int newSceneIndex)
    {
        if (!unloading)
        {
            unloading = true;
            yield return null;
            var current = SceneManager.GetActiveScene();
            //yield return FadeScreen(true);
            //LevelUnloadPreLogic((SceneNames)current.buildIndex);
            var unload = SceneManager.UnloadSceneAsync(current);
            while (!unload.isDone)
            {
                unloading = false;
                yield return null;
            }
            yield return LoadNewScene(newSceneIndex);
            //yield return FadeScreen(false);
        }
        else
        {
            Debug.Log("I shouldn't be here!");
        }
        yield return null;
    }
}
