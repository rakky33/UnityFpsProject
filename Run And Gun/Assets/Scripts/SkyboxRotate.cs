using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{
    public float RotateSpeed = 1.2f;
    void FixedUpdate()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.fixedTime * RotateSpeed);
    }
}
