using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    private bool pauseReleased = true;
    private bool isPaused;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private GameObject dontDestroyOnLoad;
    [SerializeField] private GameObject eventSystemPrefab;
    private GameObject eventSystem;
    private bool gameOver;
    
    

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(dontDestroyOnLoad);
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

    private void Start()
    {
        if (eventSystem == null)
            eventSystem = Instantiate(eventSystemPrefab, dontDestroyOnLoad.transform);
    }
    private void OnEnable()
    {
        if(playerControls != null)
            playerControls.Player.Enable();
    }
    private void OnDisable()
    {
        if(playerControls != null)
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
        ListenPause();
        ListenSkipLevel();
    }

    private void ListenPause()
    {
        if (!gameOver && pauseReleased && playerControls.Player.Pause.IsPressed())
        {
            pauseReleased = false;
            SetPause();
        }
        if (playerControls.Player.Pause.WasReleasedThisFrame())
            pauseReleased = true;
    }

    private void SetPause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        if(isPaused)
            playerCtrlr.DisableControls();
    }

    private void ListenSkipLevel()
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

    public void EndCutscene()
    {
        mainCamera.SetActive(true);
        virtualCamera.SetActive(true);
        Player.SetActive(true);
    }

    public GameObject GetEnemyGo(EnemyName enemyName)
    {
        return enemyGoAssociation.FirstOrDefault(enemy => enemy.Name == enemyName).Prefab;
    }

    public void GameOver(GameObject go)
    {
        Debug.Log("Game over");
        Time.timeScale = 0;
        Player.SetActive(false);
        endGameScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        int thisLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (String.IsNullOrWhiteSpace(SceneUtility.GetScenePathByBuildIndex(thisLevelBuildIndex)))
            return;
        SceneManager.LoadScene(thisLevelBuildIndex);
        Player.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        virtualCamera.SetActive(true);
        OnLevelLoad?.Invoke();
        if(isPaused)
            SetPause();
    }

    public void RestartGame()
    {
        if (String.IsNullOrWhiteSpace(SceneUtility.GetScenePathByBuildIndex(0)))
            return;
        SceneManager.LoadScene(0);
        Player.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        virtualCamera.SetActive(true);
        OnLevelLoad?.Invoke();
        if(isPaused)
            SetPause();
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
        endGameScreen.SetActive(false);
        if(isPaused)
            SetPause();
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
