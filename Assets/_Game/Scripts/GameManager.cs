using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject Player;
    private PlayerController playerCtrlr;
    public Vector3 playerPosition => playerCtrlr.position;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        playerCtrlr = Player.GetComponent<PlayerController>();
    }
}
