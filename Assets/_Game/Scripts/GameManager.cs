using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] 
    private GameObject Player;
    public UnityAction OnLevelLoad;
    [SerializeField] 
    private int currentScene = 0;
    private WaveController waveController;
    private PlayerController playerCtrlr;
    public List<EnemyToGo> enemyGoAssociation;
    [SerializeField] 
    private List<SceneAsset> scenes;
    public Vector3 playerPosition => playerCtrlr.position;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
        Instance = this;
        playerCtrlr = Player.GetComponent<PlayerController>();
    }

    public GameObject GetEnemyGo(EnemyName enemyName)
    {
        return enemyGoAssociation.FirstOrDefault(enemy => enemy.Name == enemyName).Prefab;
    }

    public void NextLevel()
    {
        currentScene += 1;
        if (scenes.Count <= currentScene)
            return;
        SceneManager.LoadScene(scenes[currentScene].name);
        OnLevelLoad?.Invoke();
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
