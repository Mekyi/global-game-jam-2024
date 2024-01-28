using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    //[field: Header("Ambience")]
    //[field: SerializeField]
    //public EventReference Ambience { get; private set; }


    [field: Header("Player SFX")]
    
    [field: SerializeField]
    public EventReference PlayerGun { get; private set; }

    [field: SerializeField]
    public EventReference PlayerDash { get; private set; }

    //[field: SerializeField]
    //public EventReference PlayerDamageTaken { get; private set; }

    //[field: SerializeField]
    //public EventReference PlayerDeath { get; private set; }

    
    [field: Header("Enemy")]

    [field: SerializeField]
    public EventReference EnemyProgrammerShoot { get; private set; }

    [field: SerializeField]
    public EventReference EnemyTurretPlacement{ get; private set; }

    [field: SerializeField]
    public EventReference EnemyTurretShoot { get; private set; }

    [field: SerializeField]
    public EventReference EnemyDamageTaken { get; private set; }

    [field: SerializeField]
    public EventReference EnemyDeath { get; private set; }

    //[field: SerializeField]
    //public EventReference EnemyCatMeow { get; private set; }

    //[field: SerializeField]
    //public EventReference EnemyCatAttack { get; private set; }

    //[field: SerializeField]
    //public EventReference EnemyCatDeath { get; private set; }

    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
        } 
        else 
        { 
            Instance = this; 
        } 

        Instance = this;
    }
}
