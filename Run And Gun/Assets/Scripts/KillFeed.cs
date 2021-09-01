using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeed : MonoBehaviour
{
    public GameObject killFeed;
    public Transform Spawn;
    PhotonView PV;

    string playerName;
    string enemyName;

    public Sprite[] deathTypes;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }
    public void CallOuteveryone(int dIndex, string Kstring, string GKstring)
    {
        PV.RPC("RPC_KillFeedShown", RpcTarget.All, dIndex, Kstring, GKstring);
    }

    [PunRPC]
    public void RPC_KillFeedShown(int deathType,string Kname,string GKname)
    {
        GameObject k = Instantiate(killFeed, Spawn.position, Spawn.rotation);
        k.transform.SetParent(Spawn);

        Text playerName;
        Text enemyName;

        Image dImage;

        Debug.Log(deathType + Kname + GKname);
        foreach (Transform child in k.transform)
        {
            if (child.name == "Pname")
            {
                playerName = child.GetComponent<Text>();
                playerName.text = "<color=white>" + Kname + "</color>";
            }
            else if (child.name == "Ename")
            {
                enemyName = child.GetComponent<Text>();
                enemyName.text = "<color=red>" + GKname + "</color>";
            }
            else if (child.name == "DImage")
            {
                dImage = child.GetComponent<Image>();
                dImage.sprite = deathTypes[deathType];
            }
        }

        Destroy(k, 5f);
    }
}
