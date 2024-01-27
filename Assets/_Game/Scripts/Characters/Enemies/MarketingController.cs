using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class MarketingController : EnemyBase
{
    [SerializeField] 
    private int maxTurrets;
    [SerializeField] 
    private float angleDeviation;

    private float angleMin;
    private float angleMax;
    private Vector3 nextWaypoint;
    private List<GameObject> turrets = new ();
    private float distToWaypoint;

    protected override void Awake()
    {
        base.Awake();
        angleMax = angleDeviation / 2;
        angleMin = -angleMax;
    }

    protected override void Shoot()
    {
        if (!canShoot || turrets.Count >= maxTurrets)
            return;
        var turret = Instantiate(bulletPrefab);
        turrets.Add(turret);
        turret.GetComponent<CharacterBase>().OnDeath += DestroyTurret;
        turret.transform.position = position;
        shootTimer = 1 / fireRate;
    }

    private void DestroyTurret(GameObject turret)
    {
        turrets.Remove(turret);
        turret.GetComponent<CharacterBase>().Die();
    }

    public override void Die()
    {
        foreach (var turret in turrets)
        {
            DestroyTurret(turret);
        }
        base.Die();
    }

    protected override void Move()
    {
        if (Vector3.Distance(position, nextWaypoint) < 0.1f)
        {
            GetNextWaypoint();
        }
        base.Move();
    }

    private void GetNextWaypoint()
    {
        var angle = Random.Range(angleMin, angleMax);
        lookDir = (Vector3.zero - position).normalized;
        lookDir = Quaternion.AngleAxis(angle, Vector3.up) * lookDir;
        var hit = Physics2D.Raycast(position, lookDir, 100);
        var dist = Vector3.Distance(position, hit.point);
        var distToGo = Random.Range(dist / 2, dist);
        nextWaypoint = position + distToGo * lookDir;
    }

    protected override void AimAtPlayer()
    {
    }
}
