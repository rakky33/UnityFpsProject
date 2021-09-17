using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FPS_PING_Script : MonoBehaviour
{
    public int avgFrameRate;
    public Text FPSDisplay;
    public Text PingDisplay;
    public void Update()
    {
        float current = 0;
        current = 1 / Time.deltaTime;
        avgFrameRate = (int)current;
        FPSDisplay.text = "FPS: " + avgFrameRate.ToString();
        PingDisplay.text = "PING: " + PhotonNetwork.GetPing() + " ms";
    }
}