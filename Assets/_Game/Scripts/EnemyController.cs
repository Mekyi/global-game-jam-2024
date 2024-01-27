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
    private GameObject bulletPrefab;
    private Vector3 playerPos => GameManager.Instance.playerPosition;
    [SerializeField]
    private float minShootingRange = 5f;
    [SerializeField]
    private float maxShootingRange = 10f;
    private float shootingRange;
    private float randomStraifingDirection;
    
    // Start is called before the first frame update
    void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        randomStraifingDirection = UnityEngine.Random.Range(0, 2);
        shootingRange = UnityEngine.Random.Range(minShootingRange, maxShootingRange);
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        AimAtPlayer();
        ShootTimer();
        Shoot();
    }
    private void FixedUpdate()
    {
        Move();
    }

    void Move()
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

    void AimAtPlayer()
    {
        lookDir = (playerPos - position).normalized;
    }
    private void ShootTimer()
    {
        if (canShoot)
            return;
        shootTimer -= Time.deltaTime;
    }
    
    private void Shoot()
    {
        if (!canShoot)
            return;
        var bullet = Instantiate(bulletPrefab);
        bullet.GetComponent<BulletBase>().Shoot(lookDir, position);
        shootTimer = 1 / UnityEngine.Random.Range(minFireRate, maxFireRate);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyProgrammerShoot, transform.position);
    }

    public override void DealDamage(float amount)
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyDamageTaken, transform.position);
        base.DealDamage(amount);
    }

    internal override void Death()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyDeath, transform.position);
        base.Death();
    }
}
