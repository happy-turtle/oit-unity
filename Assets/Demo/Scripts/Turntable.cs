using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class Turntable : MonoBehaviour
{
    public float turnSpeed = 1f;
    CinemachineFreeLook freeLookCam;

    void Start()
    {
        freeLookCam = GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        freeLookCam.m_XAxis.Value += turnSpeed * Time.deltaTime;
    }
}
