using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        var playerController = FindObjectOfType<PlayerController>();

        if (playerController == null)
        {
            Debug.LogWarning("MouseFollower didn't find PlayerController");
            return;
        }

        var virtualMouse = playerController.GetVirtualMouse().transform;

        _virtualCamera.Follow = virtualMouse;
        //_virtualCamera.LookAt = virtualMouse;
    }
}
