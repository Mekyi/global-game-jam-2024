using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private GameObject bulletPrefab;
    private Vector3 playerPos => GameManager.Instance.playerPosition;

    // Start is called before the first frame update
    void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
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
        rBody.velocity = lookDir * speed;
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
        shootTimer = 1 / fireRate;
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyProgrammerShoot, transform.position);
    }

    public override void DealDamage(float amount)
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyDamageTaken, transform.position);
        base.DealDamage(amount);
    }
}
