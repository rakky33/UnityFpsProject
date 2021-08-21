using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string MenuName;
    public bool open;
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }
    public void Colse()
    {
        open = false;
        gameObject.SetActive(false);
    }
}
