using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerGunSystem : MonoBehaviourPunCallbacks, IDmangeable
{

	Camera cam;

	[Header("Gun system")]
	[SerializeField] Item[] items;
	public float fireRateAK = 20f;
	private float nextTimeToFireAK = 0f;
	public float fireRatePistol = 0.5f;
	private float nextTimeToFirePistol = 0f;
	public ParticleSystem MuzzleFlash;
	public int itemIndex;
	int previousItemIndex = -1;

	[Header("Health system")]
	const float maxHealth = 100f;
	float currenthealth = maxHealth;
	[SerializeField] Image healthbarImage;
	[SerializeField] GameObject ui;

	[Header("Other")]
	PhotonView PV;
	PlayerManager playerManager;

	void Awake()
    {
		PV = GetComponent<PhotonView>();
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}
	void Start()
	{
		//destroy other object that isn't mine
		if (PV.IsMine)
		{
			EquipItem(0);
		}
		else
		{
			Destroy(ui);
		}
	}

	void Update()
	{
		
		if (!PV.IsMine)
			return;
		//You can pick Item by press 1,2
		for (int i = 0; i < items.Length; i++)
		{
			if (Input.GetKeyDown((i + 1).ToString()))
			{
				EquipItem(i);
				break;
			}
		}
		//And u can pick Item by scroll wheel
		if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			if (itemIndex >= items.Length - 1)
			{
				EquipItem(0);
			}
			else
			{
				EquipItem(itemIndex + 1);
			}
		}
		else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			if (itemIndex <= 0)
			{
				EquipItem(items.Length - 1);
			}
			else
			{
				EquipItem(itemIndex - 1);
			}
		}
		//fire AK(U can hold) ((I need to fix it that when u pick up it wait for 3 secound and it can shoot))
		if (Input.GetButton("Fire1") && Time.time >= nextTimeToFireAK)
		{
			if (itemIndex == 0)
			{
				nextTimeToFireAK = Time.time + 1f / fireRateAK;
				MuzzleFlash.Play();
				items[itemIndex].Use();
			}
		}
		//fire Pistol (U have to click)
		if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFirePistol)
		{
			if (itemIndex == 1)
			{
				nextTimeToFireAK = Time.time + 1f / fireRatePistol;
				MuzzleFlash.Play();
				items[itemIndex].Use();
			}
		}
		//If y== -30(or if u fell out of the world) u die
		if (transform.position.y < -30f)
		{
			Debug.Log("Fell die");
			Die();
		}
	}



	//Decide what item it should pick
	void EquipItem(int _index)
	{
		if (_index == previousItemIndex)
			return;

		itemIndex = _index;

		items[itemIndex].itemGameObject.SetActive(true);

		if (previousItemIndex != -1)
		{
			items[previousItemIndex].itemGameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		if (PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

		}
	}

	//Show the gun that onwer pick to other
	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (!PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

	//Get damage by network synch
	public void TakeDamage(float damage)
	{
		PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
	}

	//Make damage by network synch
	[PunRPC]
	void RPC_TakeDamage(float damage)
	{
		if (!PV.IsMine)
			return;

		currenthealth -= damage;

		healthbarImage.fillAmount = currenthealth / maxHealth;

		if (currenthealth <= 0)
		{
			Die();
		}
	}

	//Die system
	void Die()
	{
		playerManager.Die();
	}

}
