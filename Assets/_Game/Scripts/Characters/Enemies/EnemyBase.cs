using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    [SerializeField]
    internal float fireRate;
    [SerializeField]
    internal GameObject bulletPrefab;
    internal Vector3 playerPos => GameManager.Instance.playerPosition;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
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
        rBody.velocity = lookDir * speed;
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
        shootTimer = 1 / fireRate;
    }
}
