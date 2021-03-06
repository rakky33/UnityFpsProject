using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;

    private AudioSource audiosource;
    public AudioClip AKsound;
    public AudioClip Pistolsound;
    

    PlayerGunSystem whichGun;
    [SerializeField] PhotonView playerPV;

    public PlayerGunSystem StartSound;

    void Awake()
    {
        whichGun = FindObjectOfType<PlayerGunSystem>();
        audiosource = GetComponent<AudioSource>();
        PV = GetComponent<PhotonView>();
        
    }

    void Start()
    {
        
        StartSound = GetComponent<PlayerGunSystem>();
    }

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
                hit.collider.gameObject.GetComponent<IDmangeable>()?.TakeDamage(((GunInfo)itemInfo).damage);
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]    
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        if (whichGun.itemIndex == 0)
        {
            audiosource.PlayOneShot(AKsound);
        }
        else if (whichGun.itemIndex == 1)
        {
            audiosource.PlayOneShot(Pistolsound);
        }
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {           
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 5f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
    
}

