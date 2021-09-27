using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelthBoxScript : MonoBehaviour
{
    public float speed = 100f;
    [SerializeField] GameObject BoxGraphic;
    bool Active;

    private void Awake()
    {
        Active = false;
        BoxGraphic.SetActive(false);
    }

    void FixedUpdate()
    {
        if (Active == true)
        {
            transform.Rotate(0f, 0f, speed * Time.fixedDeltaTime);
        }
    }

    public void SpawnHelthBox()
    {
        Active = true;
        BoxGraphic.SetActive(true);
    }

    void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.tag.Equals("Player"))
        {
            BoxGraphic.SetActive(false);
            target.gameObject.GetComponent<PlayerGunSystem>().heal();
            Active = false;
        }
    }
}
