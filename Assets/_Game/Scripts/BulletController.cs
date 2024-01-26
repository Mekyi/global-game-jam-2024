using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float dmg;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    private Vector3 velocity;
    private Rigidbody2D rBody;
    private Vector3 startPos;
    
    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
    }
    public void Shoot(Vector3 direction, Vector3 pos)
    {
        velocity = speed * direction;
        transform.position = startPos = pos;
    }
    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, startPos) > range)
        {
            Destroy(gameObject);
            return;
        }
        rBody.velocity = velocity;
    }
}
