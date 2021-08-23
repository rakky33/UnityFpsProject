using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDmangeable
{
	
	[Header("Looking")]
	[SerializeField] GameObject cameraHolder;
	float verticalLookRotation;
	Camera cam;
	[SerializeField]
	float mouseSensitivity;

	[Header("Movement")]
	public float moveSpeed = 6f;
	public float movementMultiplier = 10f;
	[SerializeField]public float airtMultiplier = 0.15f;
	float horizontalMovement;
	float verticalMovement;
	float PlayerHeight = 4f;
	float groundDrag = 6f;
	float airDrag = 1f;
	Vector3 moveDirection;
	Rigidbody rb;

	[Header("KeyBlinds")]
	[SerializeField] KeyCode jumpKey = KeyCode.Space;

	[Header("Jumping")]
	public float jumpForce = 12f;
	bool isGrounded;

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
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		cam = GetComponentInChildren<Camera>();

		playerManager =  PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}

	void Start()
	{
		//destroy other object that isn't mine
		rb.freezeRotation = true;
		if (PV.IsMine)
		{
			EquipItem(0);
		}
		else
		{
			Destroy(ui);
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(rb);
		}
	}

	void Update()
	{

		if (!PV.IsMine)
			return;
		//Jump raycast to chewck is ground?
		isGrounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight / 2 + 0.1f);
		//Some fuction
		Look();
		MyInputWalk();
		ControlDrag();
		//If press spacebar so u will jump
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
			Debug.Log("jump");
			//Jump
			Jump();
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
			Die();
        }
	}

	//Walk Input Get W A S D movement W = 1,S = -1,A = 1,D = -1
	void MyInputWalk()
	{
		horizontalMovement = Input.GetAxisRaw("Horizontal");
		verticalMovement = Input.GetAxisRaw("Vertical");

		moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;		
	}
	//Get mouseX mouseY then controll camera and make it MathF so u cannot look over 90degree in Yaxis
	void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	//Make it not slippy when u move
	void ControlDrag()
    {
        if (isGrounded)
        {
			rb.drag = groundDrag;
        }
        else
        {
			rb.drag = airDrag;
        }
    }

	//Add force to bean when u jump
	void Jump()
    {
		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

	//break other movement so it will only move onwer client and not confuse the photon network
	private void FixedUpdate()
    {
		if (!PV.IsMine)
			return;
		MovePlayer();
    }

	//Moveplayer use force to move
	void MovePlayer()
    {
        if (isGrounded)
        {
			rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
		else if (!isGrounded)
        {
			rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airtMultiplier, ForceMode.Acceleration);	
		}
    }

	//Decide what item it should pick
	void EquipItem(int _index)
    {
		if (_index == previousItemIndex)
			return;

		itemIndex = _index;

		items[itemIndex].itemGameObject.SetActive(true);

		if(previousItemIndex != -1)
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

		if(currenthealth <= 0)
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
