using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;

public class PlayerManager : MonoBehaviour
{


    [Header("Scoreboard System")]
    public int deathsCount;

    PhotonView PV;

    GameObject controller;
    void Awake()
    {

        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    // Update is called once per frame
    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        UpdateDeaths();
        PhotonNetwork.Destroy(controller);
        CreateController();
    }


    public void UpdateDeaths()
    {
        deathsCount = deathsCount + 1;

        Debug.Log("U have Death: " + deathsCount);
    }



    public delegate void OnPlayerKilledCallback(string player, string action, string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;
}