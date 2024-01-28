using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject Player;
    public UnityAction OnLevelLoad;
    [SerializeField] 
    private int currentScene = 0;
    private WaveController waveController;
    private PlayerController playerCtrlr;
    public List<EnemyToGo> enemyGoAssociation;
    public Vector3 playerPosition => playerCtrlr.position;
    [HideInInspector]
    public bool isKeyboardAndMouse;
    public GameObject mainCamera;
    [SerializeField] 
    private GameObject virtualCamera;
    private bool skipping = false;
    private PlayerControls playerControls;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        Application.targetFrameRate = 60;
        playerCtrlr = Player.GetComponent<PlayerController>();
        playerControls = new PlayerControls();
        InputSystem.onActionChange += InputActionChangeCallback;
    }
    private void OnEnable()
    {
        playerControls.Player.Enable();
    }
    private void OnDisable()
    {
        playerControls.Player.Disable();
    }
    private void InputActionChangeCallback(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction receivedInputAction = (InputAction) obj;
            InputDevice lastDevice = receivedInputAction.activeControl.device;
 
            isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
        }
    }

    private void Update()
    {
        if (skipping)
        {
            if (playerControls.Player.SkipLevel.WasReleasedThisFrame())
                skipping = false;
            return;
        }
        if (!playerControls.Player.SkipLevel.IsPressed())
            return;
        skipping = true;
        NextLevel();
    }

    public void SetCutscene()
    {
        mainCamera.SetActive(false);
        virtualCamera.SetActive(false);
        Player.SetActive(false);
    }
    public GameObject GetEnemyGo(EnemyName enemyName)
    {
        return enemyGoAssociation.FirstOrDefault(enemy => enemy.Name == enemyName).Prefab;
    }
    public void NextLevel()
    {
        int nextLevelBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (String.IsNullOrWhiteSpace(SceneUtility.GetScenePathByBuildIndex(nextLevelBuildIndex)))
            return;
        SceneManager.LoadScene(nextLevelBuildIndex);
        Player.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        virtualCamera.SetActive(true);
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
