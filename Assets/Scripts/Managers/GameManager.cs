using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    bool managerIsCreatedThisScene = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        managerIsCreatedThisScene = true;
        InitManager();
    }

    private void InitManager()
    {
        Debug.Log("Manager was created " + managerIsCreatedThisScene);
    }
}
