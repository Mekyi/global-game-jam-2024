using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    [SerializeField]
    private float minFireRate;
    [SerializeField]
    private float maxFireRate;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    internal GameObject bulletPrefab;
    private Vector3 playerPos => GameManager.Instance.playerPosition;
    [SerializeField]
    private float minShootingRange = 5f;
    [SerializeField]
    private float maxShootingRange = 10f;
    private float shootingRange;
    private float randomStraifingDirection;

    [SerializeField]
    private EventReference shootSound;

    [SerializeField]
    private EventReference deathSound;
    
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        randomStraifingDirection = UnityEngine.Random.Range(0, 2);
        shootingRange = UnityEngine.Random.Range(minShootingRange, maxShootingRange);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        position = transform.position;
        AimAtPlayer();
        ShootTimer();
        Shoot();
    }
    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (Vector2.Distance(gameObject.transform.position, playerPos) > shootingRange)
        {
            rBody.velocity = lookDir * speed;
        }
        else
        {
            if (randomStraifingDirection == 0)
            {
                rBody.velocity = new Vector2(lookDir.y, -lookDir.x) * speed;
            }
            else
            {
                rBody.velocity = new Vector2(-lookDir.y, lookDir.x) * speed;
            }
        }
    }

    protected virtual void AimAtPlayer()
    {
        lookDir = (playerPos - position).normalized;
    }
    
    private void ShootTimer()
    {
        if (canShoot)
            return;
        shootTimer -= Time.deltaTime;
    }
    
    protected virtual void Shoot()
    {
        if (!canShoot)
            return;
        var bullet = Instantiate(bulletPrefab);
        bullet.GetComponent<BulletBase>().Shoot(lookDir, position);
        shootTimer = 1 / GetFireRate();

        if (!shootSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(shootSound, transform.position);
        }
    }

    protected float GetFireRate()
    {
        return UnityEngine.Random.Range(minFireRate, maxFireRate);
    }

    public override bool DealDamage(float amount)
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyDamageTaken, transform.position);
        return base.DealDamage(amount);
    }

    internal override void Death()
    {
        if (!deathSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(deathSound, transform.position);
        }
        base.Death();
    }
}
