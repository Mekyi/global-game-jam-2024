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

    private void Awake()
    {
        playerControls = new PlayerControls();
        rBody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
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
        mouseInput = cam.ScreenToWorldPoint(playerControls.Player.Look.ReadValue<Vector2>());
        var dir = (Vector3)mouseInput - position;
        dir.z = 0;
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
