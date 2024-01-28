using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadObjects : MonoBehaviour
{
    public static DontDestroyOnLoadObjects Instance { get; private set; }
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
        DontDestroyOnLoad(gameObject);
    }
}
