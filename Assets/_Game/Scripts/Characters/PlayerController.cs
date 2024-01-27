using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase
{
    [SerializeField] 
    private Camera cam;
    [SerializeField] 
    private PowerUp powerUp;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private bool isKeyboardAndMouse;
    private float invincibilityTimer;
    [SerializeField] 
    private float invincibleTime = 0.5f;
    private bool invincible => invincibilityTimer > 0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        playerControls = new PlayerControls();
        rBody = GetComponent<Rigidbody2D>();
        InputSystem.onActionChange += InputActionChangeCallback;
    }

    internal override void Start()
    {
        base.Start();
        GameManager.Instance.OnLevelLoad += OnLevelLoad;
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

    private void OnLevelLoad()
    {
        currentHealth = maxHealth;
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
    }

    private void Update()
    {
        position = transform.position;
        Look();
        ShootTimer();
        InvincibilityTimer();
        Shoot();
    }

    private void Move()
    {
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        rBody.velocity = moveInput.normalized * speed;
    }
    private void Look()
    {
        Vector3 dir = new Vector3();
        if (isKeyboardAndMouse)
        {
            var mouseInput = cam.ScreenToWorldPoint(playerControls.Player.AimMouse.ReadValue<Vector2>());
            dir = mouseInput - position;
            dir.z = 0;
        }
        else
        {
            dir = playerControls.Player.AimGamepad.ReadValue<Vector2>();
        }
        lookDir = dir.normalized;
    }

    private void ShootTimer()
    {
        if (canShoot)
            return;
        shootTimer -= Time.deltaTime;
    }
    private void InvincibilityTimer()
    {
        if (!invincible)
            return;
        invincibilityTimer -= Time.deltaTime;
    }
    private void Shoot()
    {
        if (!canShoot || !playerControls.Player.Shoot.IsPressed())
            return;
        var bullet = Instantiate(powerUp.bulletPrefab);
        bullet.GetComponent<BulletBase>().Shoot(lookDir, position);
        shootTimer = 1 / powerUp.fireRate;
    }
    public override void DealDamage(float amount)
    {
        if (invincible)
            return;
        invincibilityTimer = invincibleTime;
        base.DealDamage(amount);
    }

    protected override void SetGrayScale()
    {
        grayScaler?.SetColorScale(currentHealth / maxHealth);
    }
    
}
