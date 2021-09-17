using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VB_Storage : MonoBehaviour
{
    [Header("Scoreboard System")]
    public int killCount;
    public int deathsCount;

    public void VB_KillCount()
    {
        killCount = killCount + 1;

        Debug.Log("U have kill: " + killCount);
    }

    public void VB_DeathCount()
    {
        deathsCount = deathsCount + 1;

        Debug.Log("U have Death: " + deathsCount);
    }
}
