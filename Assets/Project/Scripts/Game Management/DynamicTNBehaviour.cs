using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

public class DynamicTNBehaviour : TNBehaviour
{

    protected virtual void OnEnable()
    {
        if (BattleGameManager.instance == null)
        {
            Debug.LogFormat("enable too soon for {0}", gameObject.name);
            return;
        }
       //BattleGameManager.instance.gameEvents.OnChangeHost.AddListener(onChangeHost);
    }

    protected virtual void OnDisable()
    {
        if (BattleGameManager.instance == null)
        {
            Debug.LogFormat("disable too soon for {0}", gameObject.name);
            return;
        }
        //BattleGameManager.instance.gameEvents.OnChangeHost.RemoveListener(onChangeHost);
    }

    private void onChangeHost(int playerID)
    {
        Debug.LogFormat("{0} changing owner from {1} to {2}", gameObject.name, tno.ownerID, playerID);
        tno.ownerID = playerID;
    }
}
