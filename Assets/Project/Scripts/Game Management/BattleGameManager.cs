using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;

public class BattleGameManager : TNEventReceiver
{
    public GameObject[] SpawnPoints;
    public Transform parkTransform;
    public GameEvents gameEvents;

    private int channelID;
    [RCC]
    static GameObject CreateBattlePlayer(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;

        return go;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        gameEvents.OnPlayerSpawn.AddListener(onPlayerSpawn);
        gameEvents.OnPlayerDeath.AddListener(onGameOver);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        gameEvents.OnPlayerSpawn.RemoveListener(onPlayerSpawn);
        gameEvents.OnPlayerDeath.RemoveListener(onGameOver);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public bool spawnPlayer;
    private bool playerSpawned;
    private void Update()
    {
        if (!playerSpawned && spawnPlayer)
        {
            spawnPlayer = false;
            playerSpawned = true;
            onPlayerSpawn();
        }
   
    }

    protected override void OnJoinChannel(int channelID, bool success, string message)
    {
        this.channelID = channelID;
        onPlayerSpawn();
    }

    public void onGameOver(BattlePlayer p)
    {
        ParkPlayer(p);
        StartCoroutine(RespawnClock());
    }

    public IEnumerator RespawnClock()
    {
        var respawnTime = 10f;
        while (respawnTime > 0)
        {
            respawnTime -= Time.deltaTime;
            yield return null;
        }
        gameEvents.OnPlayerSpawn.Invoke();

    }

    public void onPlayerSpawn()
    {
        SpawnPlayer(channelID);
    }

    public void ParkPlayer(BattlePlayer p)
    {
        p.transform.position = parkTransform.position;
        p.GetComponent<Rigidbody>().isKinematic = true;
    }
    public void SpawnPlayer(int channelID)
    {
        var index = Random.Range(0, SpawnPoints.Length);
        var sp = SpawnPoints[index];
        var p = GameObject.FindGameObjectsWithTag("Player");
        BattlePlayer found = null;
        foreach (var player in p)
        {
            var bp = player.GetComponent<BattlePlayer>();
            if (bp.tno.isMine)
            {
                found = bp;
            }
        }
        if (!found)
        {
            TNManager.Instantiate(channelID, "CreateBattlePlayer", "Prefabs/Player/Battle Network Player", false, sp.transform.position, sp.transform.rotation);
        }
        else
        {
            found.transform.position = sp.transform.position;
            found.transform.rotation = sp.transform.rotation;
            found.GetComponent<Rigidbody>().isKinematic = false;
            found.Respawn();
        }
    }
}
