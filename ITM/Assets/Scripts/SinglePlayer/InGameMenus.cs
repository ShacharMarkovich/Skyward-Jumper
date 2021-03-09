using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manage the in game menus
/// </summary>
public class InGameMenus : MonoBehaviour
{
    public GameObject _pauseMenu;
    public GameObject _gameOverMenu;
    public GameObject _endFirstButton; // in Game Over Menu

    public  TextMeshProUGUI _gameOverText;
    public  AudioSource[] _gameOverAudio;
    private EventSystem _EventSystem;
    private bool _IsPaused;

    private void Start()
    {
        _EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        _IsPaused = false;
    }

    void Update()
    {
        if (!_gameOverMenu.GetActive() && Input.GetKeyDown(KeyCode.Escape))
        {
            if (_IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void ShowGameOverMenu()
    {
        Time.timeScale = 0f;
        _gameOverMenu.SetActive(true);

        _EventSystem.SetSelectedGameObject(_endFirstButton, null);
    }

    /// <summary>
    /// Called from multiplayer.
    /// change the gameover text and play the correct sound
    /// </summary>
    /// <param name="isWinner">if the player win it = 1, else = 0</param>
    public void ShowGameOverMenu(bool isWinner)
    {
        if (isWinner)
        {
            _gameOverText.text = "You Win!";
            _gameOverAudio[(int)Sounds.WinSound].Play();
        }
        else
        {
            _gameOverText.text = "You Lose!";
            _gameOverAudio[(int)Sounds.LoseSound].Play();
        }
        ShowGameOverMenu();
    }

    private void Resume()
    {
        _pauseMenu.SetActive(false);
        _EventSystem.SetSelectedGameObject(GameObject.Find("MenuButton"), null);

        Time.timeScale = 1f;
        _IsPaused = false;
    }

    private void Pause()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        _IsPaused = true;
    }

    public void StartNewGame()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex == 2) // multiplayer
            if(PhotonNetwork.inRoom)
                PhotonNetwork.LeaveRoom();

        SceneManager.LoadScene(buildIndex);
    }

    public void LoadMenu()
    {
        PhotonNetwork.LeaveRoom();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool IsGameOver()
    {
        if (_gameOverMenu.GetActive())
            return true;
        else
            return false;
    }
}