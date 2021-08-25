using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Die : MonoBehaviour
{
    PlayerManager playerManager;
    PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
