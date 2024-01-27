using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private PlayerControls playerControls;
    private Rigidbody2D rBody;
    private Vector2 moveInput;
    private Vector2 mouseInput;
    [SerializeField] 
    private Camera cam;
    private Vector3 lookDir;
    [SerializeField] 
    private PowerUp powerUp;
    private float shootTimer;
    private bool canShoot => shootTimer <= 0f;
    private Vector3 position;
    private Gamepad gamePad;
    private bool isKeyboardAndMouse;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rBody = GetComponent<Rigidbody2D>();
        InputSystem.onActionChange += InputActionChangeCallback;
    }
    private void InputActionChangeCallback(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction receivedInputAction = (InputAction) obj;
            InputDevice lastDevice = receivedInputAction.activeControl.device;

            isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
        }
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        gamePad = Gamepad.current;
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void FixedUpdate()
    {
        Move();
        Look();
    }

    private void Update()
    {
        ShootTimer();
        Shoot();
    }

    private void Move()
    {
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        rBody.velocity = moveInput.normalized * speed;
    }
    private void Look()
    {
        position = transform.position;
        Vector3 dir = new Vector3();
        if (isKeyboardAndMouse)
        {
            mouseInput = cam.ScreenToWorldPoint(playerControls.Player.AimMouse.ReadValue<Vector2>());
            dir = (Vector3)mouseInput - position;
            dir.z = 0;
        }
        else
        {
            dir = playerControls.Player.AimGamepad.ReadValue<Vector2>();
            Debug.Log(playerControls.Player.AimGamepad.ReadValue<Vector2>());
        }
        lookDir = dir.normalized;
    }

    private void ShootTimer()
    {
        if (canShoot)
            return;
        shootTimer -= Time.deltaTime;
    }
    private void Shoot()
    {
        if (!canShoot || !playerControls.Player.Shoot.IsPressed())
            return;
        var bullet = Instantiate(powerUp.bulletPrefab);
        bullet.GetComponent<BulletController>().Shoot(lookDir, position);
        shootTimer = 1 / powerUp.fireRate;
        Debug.Log("PEW");
    }
    
}
