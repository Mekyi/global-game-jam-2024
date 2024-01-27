using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase
{
    [SerializeField] 
    private Camera cam;
    [SerializeField] 
    private PowerUp powerUp;
    [SerializeField] 
    private Animator animator;
    [SerializeField] 
    private Transform shootingPoint;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private bool isKeyboardAndMouse;
    private float invincibilityTimer;
    [SerializeField] 
    private float invincibleTime = 0.5f;
    private bool invincible => invincibilityTimer > 0f;
    private bool dodgeRolling = false;
    private float dodgeRollTimer;
    [SerializeField] 
    private float dodgeRollCooldown = 0.3f;
    private bool canDodgeRoll  => dodgeRollTimer <= 0f && !dodgeRolling;

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
        grayScaler?.SetColorScale(1);
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
        Roll();
        ShootTimer();
        InvincibilityTimer();
        DodgeRollTimer();
        Shoot();
    }

    private void Move()
    {
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        rBody.velocity = moveInput.normalized * speed;
        animator.SetBool("IsMoving", moveInput != Vector2.zero);
    }
    
    private void Roll()
    {
        if (!canDodgeRoll || !playerControls.Player.DodgeRoll.IsPressed())
            return;
        Debug.Log("Start DodgeRoll");
        dodgeRolling = true;
        animator.SetTrigger("Roll");
    }

    public void OnDodgeRollEnd()
    {
        dodgeRolling = false;
        dodgeRollTimer = dodgeRollCooldown;
        Debug.Log("Dodge roll ended");
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

    private void DodgeRollTimer()
    {
        if(canDodgeRoll)
            return;
        dodgeRollTimer -= Time.deltaTime;
    }
    private void Shoot()
    {
        if (!canShoot || !playerControls.Player.Shoot.IsPressed())
            return;
        var bullet = Instantiate(powerUp.bulletPrefab);
        bullet.GetComponent<BulletBase>().Shoot(lookDir, shootingPoint.position);
        shootTimer = 1 / powerUp.fireRate;
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerGun, transform.position);
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
