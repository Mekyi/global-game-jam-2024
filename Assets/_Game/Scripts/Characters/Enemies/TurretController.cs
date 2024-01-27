using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : EnemyBase
{
    [SerializeField] private float lifeTime;
    protected override void Awake() { }

    protected override void FixedUpdate() { }
    protected override void Update()
    {
        base.Update();
        Timer();
    }

    private void Timer()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime > 0)
            return;
        Death();
    }
}
