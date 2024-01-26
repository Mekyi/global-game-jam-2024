using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    
    [field: SerializeField]
    public EventReference PlayerShoot { get; private set; }

    [field: SerializeField]
    public EventReference PlayerDamageTaken { get; private set; }

    [field: SerializeField]
    public EventReference PlayerDeath { get; private set; }

    
    [field: Header("Enemy")]

    [field: SerializeField]
    public EventReference EnemyProgrammerShoot { get; private set; }

    [field: SerializeField]
    public EventReference EnemyTurretShoot { get; private set; }

    [field: SerializeField]
    public EventReference EnemyDeath { get; private set; }

    [field: SerializeField]
    public EventReference EnemyCatMeow { get; private set; }

    [field: SerializeField]
    public EventReference EnemyCatAttack { get; private set; }

    [field: SerializeField]
    public EventReference EnemyCatDeath { get; private set; }

    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }

        Instance = this;
    }
}
