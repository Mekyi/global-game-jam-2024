using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : CharacterBase
{
    [Header("Movement Zone")]
    [SerializeField] private Vector2 topLeftCorner;
    [SerializeField] private Vector2 bottomRightCorner;

    [Header("Movement Timing")]
    [SerializeField] private float minTimeAtPoint = 4.0f;
    [SerializeField] private float maxTimeAtPoint = 8.0f;

    [Header("Shooting Patterns")]
    [SerializeField] private float spiralRate = 0.1f;
    [SerializeField] private int spiralBulletsPerShot = 50;
    [SerializeField] private float coneRate = 0.2f;
    [SerializeField] private int coneAngle = 60;
    [SerializeField] private int coneBulletsPerShot = 50;
    [SerializeField] private float machineGunRate = 0.05f;
    [SerializeField] private int machineGunBulletsPerShot = 50;

    [Header("Mechanics")]
    [SerializeField] private GameObject firstWaveAdds;
    [SerializeField] private int firstWaveSpawnPercent = 50;
    private bool firstWaveActivated = false;
    [SerializeField] private GameObject secondWaveAdds;
    [SerializeField] private int secondWaveSpawnPercent = 25;
    private bool secondWaveActivated = false;
    [SerializeField] private int enragePercent = 33;
    private bool isEnraged = false;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject bulletPrefab;

    private Vector3 playerPos => GameManager.Instance.playerPosition;
    private Vector3 nextPosition;
    private float moveTimer;
    private bool isMoving;
    private bool readyToShoot = true;


    void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        ChooseNextPosition();
        slider.maxValue = maxHealth;
    }


    void Update()
    {
        position = transform.position;
        AimAtPlayer();

        if (!isMoving)
        {
            rBody.velocity = Vector2.zero;
        }

        animator.SetBool("IsMoving", isMoving);
    }


    private void FixedUpdate()
    {
        if (isMoving)
        {
            Move();
        }
        else
        {
            if (readyToShoot)
            {
                StartShooting();
                readyToShoot = false;
            }

            moveTimer -= Time.fixedDeltaTime;
            if (moveTimer <= 0)
            {
                ChooseNextPosition();
                readyToShoot = true;
            }
        }
    }


    private void ChooseNextPosition()
    {
        nextPosition = new Vector3(
            Random.Range(topLeftCorner.x, bottomRightCorner.x),
            Random.Range(bottomRightCorner.y, topLeftCorner.y),
            0
        );

        isMoving = true;
    }


    void Move()
    {
        Vector2 direction = (nextPosition - position).normalized;
        float distanceToTarget = Vector2.Distance(position, nextPosition);

        if (distanceToTarget < 1f)
        {
            rBody.velocity = direction * speed * (distanceToTarget / 1f);

            if (distanceToTarget < 0.1f)
            {
                isMoving = false;
                moveTimer = Random.Range(minTimeAtPoint, maxTimeAtPoint);
            }
        }
        else
        {
            rBody.velocity = direction * speed;
        }
    }


    void AimAtPlayer()
    {
        lookDir = (playerPos - position).normalized;
    }


    void StartShooting()
    {
        int pattern1 = Random.Range(0, 3);
        StartCoroutine(StartPattern(pattern1));

        if (isEnraged)
        {
            int pattern2;
            do { pattern2 = Random.Range(0, 3); }
            while (pattern2 == pattern1); // Ensure a different pattern for the second attack

            StartCoroutine(StartPattern(pattern2));
        }
    }


    IEnumerator StartPattern(int pattern)
    {
        switch (pattern)
        {
            case 0:
                yield return StartCoroutine(SpiralShoot());
                break;
            case 1:
                yield return StartCoroutine(ConeShoot());
                break;
            case 2:
                yield return StartCoroutine(MachineGunShoot());
                break;
        }
    }


    IEnumerator SpiralShoot()
    {
        float angleStep = 360f / spiralBulletsPerShot;
        float angle = 0f;

        for (int i = 0; i < spiralBulletsPerShot; i++)
        {
            var bullet = Instantiate(bulletPrefab);
            Vector2 shootDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
            bullet.GetComponent<BulletBase>().Shoot(shootDirection, position);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyProgrammerShoot, transform.position);
            angle += angleStep;

            yield return new WaitForSeconds(spiralRate);
        }
    }


    IEnumerator ConeShoot()
    {
        for (int i = 0; i < coneBulletsPerShot; i++)
        {
            Vector2 lookDir = (playerPos - position).normalized;
            float startAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - coneAngle / 2;
            var bullet = Instantiate(bulletPrefab);
            float angle = startAngle + Random.Range(0f, coneAngle);
            Vector2 shootDirection = Quaternion.Euler(0, 0, angle) * Vector2.right;
            bullet.GetComponent<BulletBase>().Shoot(shootDirection, position);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyProgrammerShoot, transform.position);

            yield return new WaitForSeconds(coneRate);
        }
    }


    IEnumerator MachineGunShoot()
    {
        for (int i = 0; i < machineGunBulletsPerShot; i++)
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<BulletBase>().Shoot(lookDir, position);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyProgrammerShoot, transform.position);

            yield return new WaitForSeconds(machineGunRate);
        }
    }


    public override bool DealDamage(float amount)
    {
        bool damageResult = base.DealDamage(amount);
        float healthPercentage = (currentHealth / maxHealth) * 100;

        if (healthPercentage <= firstWaveSpawnPercent && !firstWaveActivated)
        {
            firstWaveAdds.SetActive(true);
            firstWaveActivated = true;
        }

        if (healthPercentage <= secondWaveSpawnPercent && !secondWaveActivated)
        {
            secondWaveAdds.SetActive(true);
            secondWaveActivated = true;
        }

        if (healthPercentage <= enragePercent && !isEnraged)
        {
            isEnraged = true;
        }

        slider.value = currentHealth;

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyDamageTaken, transform.position);
        return damageResult;
    }


    internal override void Death()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyDeath, transform.position);
        base.Death();
    }
}
