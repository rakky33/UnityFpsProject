using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class healthBoxManager : MonoBehaviour
{
    [SerializeField] HelthBoxScript[] healthBox;
    int RandomRange;
    [SerializeField] PhotonView PV;
    int itemCount;

    void Start()
    {
        StartCoroutine(RandomHelthBox());
    }
    IEnumerator RandomHelthBox()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            itemCount = GameObject.FindGameObjectsWithTag("HealthBox").Length;
            Debug.Log("There is " + itemCount + " healthBox in this map");
            if(itemCount < 5)
            {
                Debug.Log("health Box system start");
                RandomRange = Random.Range(0, healthBox.Length);
                yield return new WaitForSeconds(Random.Range(0.1f, 15f));
                PV.RPC("RecieveVariableHB", RpcTarget.All, RandomRange);
                StartCoroutine(RandomHelthBox());
            }
            else
            {
                Debug.Log("Too many item in map");
                yield return new WaitForSeconds(Random.Range(0.1f, 15f));
                StartCoroutine(RandomHelthBox());
            }           
        }
    }

    [PunRPC]
    public void RecieveVariableHB(int healthint)
    {
        healthBox[healthint].GetComponent<HelthBoxScript>().SpawnHelthBox();
    }
}
