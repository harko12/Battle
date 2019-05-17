using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using System;

public class ExporterImporter : MonoBehaviour
{
    public bool Export;
    // Update is called once per frame
    void Update()
    {
        if (Export)
        {
            var obbjs = new TNet.List<TNObject>();
            obbjs.Add(gameObject.GetComponent<TNObject>());
            TNManager.ExportObjects(obbjs, delegate (DataNode n){
                Debug.Log("exported" + n.ToString());
            });
            Export = false;
        }
    }

    public static void  onExport(DataNode n) {
        Debug.Log("exported" + n.ToString());
    }
}
