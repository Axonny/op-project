using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : Singleton<UISystem>
{
    public Slider healthBar;
    public Slider manaBar;
    public Text lvlInfo;
    public GameObject menuPanel;
    public GameObject hub;

    private GameManager gameManager;

    private void Start()
    {
        var input = InputSystem.Instance.Input;
        gameManager = GameManager.Instance;

        input.Player.Escape.performed += ctx =>
        {
            if (menuPanel.activeSelf)
                ResumeGame();
            else
                PauseGame();
        };
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        hub.SetActive(true);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        menuPanel.SetActive(true);
        hub.SetActive(false);
        Time.timeScale = 0f;
    }
}
