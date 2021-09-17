using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ziplineScript : MonoBehaviour
{
    [SerializeField] private ziplineScript targetZip;
    [SerializeField] private float zipSpeed = 8f;
    [SerializeField] private float zipScale = 0.2f;

    [SerializeField] private float arrivalThreshold = 0.4f;
    [SerializeField] private LineRenderer cable;

    public Transform zipTransform;

    public bool zipping = false;
    private GameObject localZip;

    private void Awake()
    {
        cable.SetPosition(0, zipTransform.position);
        cable.SetPosition(1, targetZip.zipTransform.position);
    }
    
    private void FixedUpdate()
    {
        if (!zipping || localZip == null) return;

        localZip.GetComponent<Rigidbody>().AddForce((targetZip.zipTransform.position - zipTransform.position).normalized * zipSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);

        if(Vector3.Distance(localZip.transform.position, targetZip.zipTransform.position) <= arrivalThreshold)
        {
            ResetZipline();
        }
    }
    public void StartZipline(GameObject player)
    {
        if (zipping) return;

        localZip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        localZip.transform.position = zipTransform.position;
        localZip.transform.localScale = new Vector3(zipScale, zipScale, zipScale);
        localZip.AddComponent<Rigidbody>().useGravity = false;
        localZip.GetComponent<Collider>().isTrigger = true;

        player.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.parent = localZip.transform;
        zipping = true;
    }

    private void ResetZipline()
    {
        if (!zipping) return;

        GameObject player = localZip.transform.GetChild(0).gameObject;
        player.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.parent = null;
        Destroy(localZip);
        localZip = null;
        zipping = false;
        Debug.Log("Zipping reset");
    }
}
