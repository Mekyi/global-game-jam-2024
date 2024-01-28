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
    [SerializeField] 
    private LayerMask raycastLayer;

    private float angleMin;
    private float angleMax;
    private Vector3 nextWaypoint;
    private List<GameObject> turrets = new ();
    private float distToWaypoint;
    private float timeToWaypoint;
    

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
        shootTimer = 1 / GetFireRate();
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
            turret.GetComponent<CharacterBase>().Die();
        }
        turrets.Clear();
        base.Die();
    }

    protected override void Update()
    {
        base.Update();
        if (timeToWaypoint <= 0)
        {
            GetNextWaypoint();
        }
        WaypointTimer();
    }

    private void WaypointTimer()
    {
        timeToWaypoint -= Time.deltaTime;
    }

    protected override void Move()
    {
        rBody.velocity = lookDir * speed;
    }

    private void GetNextWaypoint()
    {
        var angle = Random.Range(angleMin, angleMax);
        lookDir = (Vector3.zero - position).normalized;
        lookDir = Quaternion.AngleAxis(angle, Vector3.forward) * lookDir;
        var hit = Physics2D.Raycast(position, lookDir, 100,raycastLayer.value);
        var dist = Vector3.Distance(position, hit.point);
        var distToGo = Random.Range(dist / 2, dist);
        timeToWaypoint =  distToGo / speed;
        nextWaypoint = position + distToGo * lookDir;
    }

    protected override void AimAtPlayer()
    {
    }
}
