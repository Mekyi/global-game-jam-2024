using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject Player;
    private WaveController waveController;
    private PlayerController playerCtrlr;
    public List<EnemyToGo> EnemyGoAssociation;
    public Vector3 playerPosition => playerCtrlr.position;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        playerCtrlr = Player.GetComponent<PlayerController>();
    }

    public GameObject GetEnemyGo(EnemyName enemyName)
    {
        return EnemyGoAssociation.FirstOrDefault(enemy => enemy.Name == enemyName).Prefab;
    }
    
}
public enum EnemyName
{
    Programmer,
    Marketing,
    Hr,
    ItForces
}

[Serializable]
public struct EnemyToGo
{
    public EnemyName Name;
    public GameObject Prefab;
}
