using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private bool isKeyboardAndMouse => GameManager.Instance.isKeyboardAndMouse;
    private float invincibilityTimer;
    [SerializeField] 
    private float invincibleTime = 0.5f;
    private bool invincible => invincibilityTimer > 0f;
    private bool dodgeRollInvincibility;
    private bool dodgeRolling = false;
    private float dodgeRollTimer;
    [SerializeField] 
    private float dodgeRollCooldown = 0.3f;
    [SerializeField] 
    private float dodgeRollSpeedMultiplierBefore = 1.5f;
    [SerializeField] 
    private float dodgeRollSpeedMultiplierAfter = 0.8f;
    [SerializeField] 
    private GameObject gun;
    
    private bool canDodgeRoll  => dodgeRollTimer <= 0f && !dodgeRolling && moveInput != Vector2.zero;


    [SerializeField]
    private float virtualMouseMaxDistance = 10f;

    private GameObject virtualMouse;

    [SerializeField]
    private GameObject healthBar;

    private Slider healthBarSlider;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rBody = GetComponent<Rigidbody2D>();
        healthBarSlider = healthBar.GetComponent<Slider>();

        CreateVirtualMouse();
    }

    internal override void Start()
    {
        base.Start();
        GameManager.Instance.OnLevelLoad += OnLevelLoad;
        OnDeath += GameManager.Instance.GameOver;
        grayScaler?.SetColorScale(1);

        SetHealthBar();
    }

    private void OnLevelLoad()
    {
        Heal(maxHealth);
        cam = GameManager.Instance.mainCamera.GetComponent<Camera>();
        if(!playerControls.Player.enabled)
            EnableControls();
        SetHealthBar();
        //CreateVirtualMouse();
    }

    public void DisableControls()
    {
        playerControls.Player.Disable();
    }
    public void EnableControls()
    {
        playerControls.Player.Enable();
        

        //CreateVirtualMouse();
    }
    

    private void OnEnable()
    {
        EnableControls();
    }

    private void OnDisable()
    {
        DisableControls();
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
        if (!dodgeRolling && dodgeRollTimer <= 0)
        {
            rBody.velocity = moveInput.normalized * speed;
            animator.SetBool("IsMoving", moveInput != Vector2.zero);
        }
    }
    
    private void Roll()
    {
        if (!canDodgeRoll || !playerControls.Player.DodgeRoll.IsPressed())
            return;
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerDash, position);
        dodgeRollInvincibility = true;
        rBody.velocity = moveInput.normalized * (dodgeRollSpeedMultiplierBefore * speed);
        dodgeRolling = true;
        animator.SetTrigger("Roll");
    }

    public void OnDodgeRollEnd()
    {
        dodgeRolling = false;
        dodgeRollInvincibility = false;
        rBody.velocity = moveInput.normalized * (dodgeRollSpeedMultiplierAfter * speed);
        dodgeRollTimer = dodgeRollCooldown;
        Debug.Log("Dodge roll ended");
    }
    private void Look()
    {
        Vector3 dir = new Vector3();
        //virtualMouse.transform.position = transform.position;

        if (isKeyboardAndMouse)
        {
            var mouseInput = cam.ScreenToWorldPoint(playerControls.Player.AimMouse.ReadValue<Vector2>());
            dir = mouseInput - position;
            dir.z = 0;

            // WIP: Look ahead through virtual mouse
            //var distanceVector = new Vector3(
            //    Mathf.Clamp(dir.x, -virtualMouseMaxDistance, virtualMouseMaxDistance), 
            //    Mathf.Clamp(dir.y, -virtualMouseMaxDistance, virtualMouseMaxDistance), 
            //    0f
            //);


            //distanceVector = distanceVector.normalized;
            //virtualMouse.transform.position = transform.position + distanceVector;
        }
        else
        {
            dir = playerControls.Player.AimGamepad.ReadValue<Vector2>();
        }
        lookDir = dir.normalized;
        SetGunRotation();
    }

    private void SetGunRotation()
    {
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90;
        gun.transform.rotation = Quaternion.Euler(0, 0, angle);
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
        bullet.GetComponent<BulletBase>().Shoot(lookDir == Vector3.zero ?( moveInput == Vector2.zero ? Vector3.right : moveInput) : lookDir, shootingPoint.position);
        shootTimer = 1 / powerUp.fireRate;
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerGun, transform.position);
    }
    public override bool DealDamage(float amount)
    {
        if (invincible || dodgeRollInvincibility)
            return false;
        invincibilityTimer = invincibleTime;
        SetHealthBar();
        return base.DealDamage(amount);
    }

    protected override void SetGrayScale()
    {
        grayScaler?.SetColorScale(currentHealth / maxHealth);
    }
    
    private void CreateVirtualMouse()
    {
        virtualMouse = new GameObject();
        virtualMouse.name = "VirtualMouse";
    }
    public GameObject GetVirtualMouse()
    {
        return virtualMouse;
    }

    private void SetHealthBar()
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;
    }
}
