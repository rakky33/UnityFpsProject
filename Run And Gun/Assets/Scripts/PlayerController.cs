using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{

	Camera cam;

	[Header("Other")]
	PhotonView PV;
	PlayerManager playerManager;
    Rigidbody rb;

	PlayerController _PlayerController;
	PlayerMovement _PlayerMovement;

    void Awake()
	{
		_PlayerController = FindObjectOfType<PlayerController>();
		_PlayerMovement = FindObjectOfType<PlayerMovement>();
		rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
		cam = GetComponentInChildren<Camera>();
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}
	
		
	void Start()
	{
		
		//destroy other object that isn't mine


		if (PV.IsMine)
		{
			
		}
		else 
		{
			if (rb == null)
				return;
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
			Destroy(_PlayerController.GetComponent<PlayerController>());
			Destroy(_PlayerMovement.GetComponent<PlayerMovement>());

			/*this.GetComponent<PlayerController>().enabled = false;
            this.GetComponent<PlayerMovement>().enabled = false;*/
		}
	}

	void Update()
	{
	//If y== -30(or if u fell out of the world) u die
		if (transform.position.y < -30f)
        {
			Debug.Log("Fell die");
			Die();
        }
	}
	void Die()
	{
		playerManager.Die();
	}
}
