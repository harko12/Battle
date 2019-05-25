using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneNames { connect, Game, GameUI};
public class GameSceneManager : MonoBehaviour
{
    public GameEvents gameEvents;
    private void Start()
    {
        var uiScene = SceneManager.GetSceneByName(SceneNames.GameUI.ToString());
        if (uiScene != null)
        {
            StartCoroutine(LoadNewScene(SceneNames.GameUI.ToString()));
        }
    }

    public IEnumerator LoadNewScene(string SceneName)
    {
        var load = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
        while (!load.isDone)
        {
            yield return null;
        }
       // gameEvents.OnPlayerSpawn.Invoke();
    }

    public static void ReturnToConnect()
    {
        SceneManager.LoadScene(SceneNames.connect.ToString());
    }


    private void OnEnable()
    {
        //gameEvents.OnPlayerDeath.AddListener(onGameOver);
    }

    private void OnDisable()
    {
        //gameEvents.OnPlayerDeath.RemoveListener(onGameOver);
    }
    /*
    public void onGameOver(BattlePlayer p)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    */
}
