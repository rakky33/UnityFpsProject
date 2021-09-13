using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scoreboard_Script : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text NumberList;
    [SerializeField] TMP_Text KillDisplay;
    [SerializeField] TMP_Text DeathsDisplay;
    [SerializeField] TMP_Text Username;
    public Player player;
    public void Setup(Player _player,int NumberList_)
    {
        player = _player;
        Username.text = _player.NickName;
        NumberList.text = NumberList_.ToString();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
