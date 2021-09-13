using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerGunSystem : MonoBehaviourPunCallbacks, IDmangeable
{

	Camera cam;


	[Header("Gun system")]
	[SerializeField] Item[] items;
	public float fireRateAK = 20f;
	private float nextTimeToFireAK = 0f;
	public ParticleSystem MuzzleFlash;
	public int itemIndex;
	int previousItemIndex = -1;

	public TMP_Text AmmoDisplay;
	int AKammo;
	int MaxAkammo = 30;
	int Pistolammo;
	int MaxPistolammo = 9;
	bool reloading;



	[Header("Health system")]
	const float maxHealth = 100f;
	float currenthealth = maxHealth;
	[SerializeField] Image healthbarImage;
	[SerializeField] GameObject ui;

	[Header("Scoreborad Sysem")]
	[SerializeField] TMP_Text Kill_DeathDisplay;
	[SerializeField] GameObject KDGO;

	[Header("Other")]
	private AudioSource audiosource;
	public AudioClip Killsound;
	PhotonView PV;
	PlayerManager playerManager;
	string GotShootName;
	[SerializeField] PhotonView playerPV;
	KillFeed killFeedScript;

	public int Gun;

	public Animator AKanimator;
	public Animator Pistolanimator;

	public GameObject glasses;

	/**
	** ItemIndex **
	0 = AK
	1 = Pistol
	*/

	void Awake()
	{
		audiosource = GetComponent<AudioSource>();
		PV = GetComponent<PhotonView>();
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
		PhotonNetwork.NickName = PlayerPrefs.GetString("username");
	}
	void Start()
	{
		AKanimator.SetBool("isReload", false);
		Pistolanimator.SetBool("isReload", false);
		reloading = false;
		AKammo = MaxAkammo;
		Pistolammo = MaxPistolammo;

		killFeedScript = FindObjectOfType<KillFeed>();
		GotShootName = PhotonNetwork.NickName;
		//destroy other object that isn't mine
		if (PV.IsMine)
		{
			EquipItem(0);
			Destroy(glasses);
		}
		else
		{
			Destroy(KDGO);
			Destroy(ui);
		}
	}

	void Update()
	{
		if (!PV.IsMine)
			return;
		int KillCount = playerManager.killCount;
		if (PV.IsMine)
		{
			Kill_DeathDisplay.text = "Kill:" + KillCount + "/Deaths:" + playerManager.deathsCount;
		}

		if (itemIndex == 0)
		{
			AmmoDisplay.text = AKammo + "/" + MaxAkammo;
		}
		else if (itemIndex == 1)
		{
			AmmoDisplay.text = Pistolammo + "/" + MaxPistolammo;
		}


		if (AKammo != MaxAkammo || Pistolammo != MaxPistolammo)
		{
			if (Input.GetKey(KeyCode.R) && !reloading)
			{
				StartCoroutine(reload());
			}
		}


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
		if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f && !reloading)
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
		else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f && !reloading)
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
		if (itemIndex == 0)
		{
			//fire AK(U can hold) ((I need to fix it that when u pick up it wait for 3 secound and it can shoot))
			if (Input.GetButton("Fire1") && Time.time >= nextTimeToFireAK && AKammo > 0 && !reloading)
			{ 
				Debug.Log("Shoot AK");
			
					AKammo -= 1;
					nextTimeToFireAK = Time.time + 1f / fireRateAK;
					MuzzleFlash.Play();
					items[itemIndex].Use();
				}
			else if (AKammo <= 0)
			{
				StartCoroutine(reload());
			}
		}

		if (itemIndex == 1)
		{
			//fire Pistol (U have to click)
            if (Input.GetButtonDown("Fire1") && Pistolammo > 0 && !reloading)
            {
				Debug.Log("Shoot Pistol");
				Pistolammo -= 1;
				MuzzleFlash.Play();
				items[itemIndex].Use();
			}
			else if (Pistolammo <= 0)
			{
				StartCoroutine(reload());
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
	public void RPC_TakeDamage(float damage, PhotonMessageInfo info)
	{
		if (!PV.IsMine)
			return;

		currenthealth -= damage;

		healthbarImage.fillAmount = currenthealth / maxHealth;

		string shootplayer = info.Sender.NickName;
		if (currenthealth <= 0)
		{
			if (itemIndex == 0)
			{
				Gun = 1;
			}
			else if (itemIndex == 1)
			{
				Gun = 2;
			}

			killFeedScript.CallOuteveryone(Gun, shootplayer, GotShootName);
			PV.RPC("RPC_CheckKill", RpcTarget.All, shootplayer);
			Die();
		}
	}


	//Die system
	void Die()
	{

		playerManager.Die();
	}

	IEnumerator reload()
	{
		if (itemIndex == 0)
		{
			AKanimator.SetBool("isReload", true);
			reloading = true;
			Debug.Log("reloading AK for 0.8f sec");
			yield return new WaitForSeconds(1.2f);
			AKammo = MaxAkammo;
			reloading = false;
			AKanimator.SetBool("isReload", false);
		}
		else if (itemIndex == 1)
		{
			Pistolanimator.SetBool("isReload", true);
			reloading = true;
			Debug.Log("reloading Pisol for 1.5f sec");
			yield return new WaitForSeconds(0.8f);
			Pistolammo = MaxPistolammo;
			reloading = false;
			Pistolanimator.SetBool("isReload", false);
		}
	}


	[PunRPC]
	public void RPC_CheckKill(string Killname)
	{
		if (Killname == GotShootName)
		{
			audiosource.PlayOneShot(Killsound);
			Debug.Log("U kill");
			playerManager.UpdateKill();
		}
	}
}