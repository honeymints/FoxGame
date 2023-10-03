using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverMenu;
    
    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += EnableGameOverMenu; //wtf is that
    }
    private void OnDisable()
    {
        Debug.Log("Hello gameover is disabled");
        PlayerController.OnPlayerDeath -= EnableGameOverMenu;
    }
    



    public void EnableGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }
     
}
