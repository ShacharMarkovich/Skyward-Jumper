using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.EventSystems;

public class MenusManager : MonoBehaviour
{
    #region Class Variables

    public AudioSource _selectAudio;
    public AudioSource _moveAudio;

    // variable for Sound Menu
    public AudioMixer _MusicMixer;
    public AudioMixer _SoundMixer;
    public Slider _sound; //hold the component of the sound that 
    public Slider _music;

    // variable for Graphic Menu
    public GameObject[] _playerSprites;
    public TextMeshProUGUI _FullScreenText;
    private Resolution[] _resolutions;
    private const int _minWidth = 640, _minHeight = 400;
    #endregion

    /// initialization function
    void Start()
    {
        _resolutions = Screen.resolutions;

        // disabled Cursor 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // set current Screen mod
        _FullScreenText.SetText(Screen.fullScreen.ToString());

        // set currnt Player sprite
        string PlayerSprite = PlayerPrefs.GetString("PlayerSprites", "Robot");
        for (int i = 0; i < 4; ++i)
        {
            if (_playerSprites[i].name == PlayerSprite)
                _playerSprites[i].SetActive(true);
            else
                _playerSprites[i].SetActive(false);
        }

        // set current volume
        float volume;
        volume = PlayerPrefs.GetFloat("SoundVolume", 0);
        _MusicMixer.SetFloat("SoundVolume", PlayerPrefs.GetFloat("SoundVolume", 0));
        _sound.value = volume;

        volume = PlayerPrefs.GetFloat("MusicVolume", 0);
        _MusicMixer.SetFloat("MusicVolume", volume);
        _music.value = volume;
    }

    /// <summary>
    /// play the currect sound if the player select a button or move between them
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            _selectAudio.Play();
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Keypad4))
            _moveAudio.Play();
    }

    /// <summary>
    /// Change the character sprite, save the current one on data and show it.
    /// </summary>
    public void ChangeCharacterSprite()
    {
        for (int i = 0; i < 4; ++i)
        {
            if (_playerSprites[i].activeSelf)
            {
                _playerSprites[i].SetActive(false);
                _playerSprites[(i + 1) % 4].SetActive(true);

                PlayerPrefs.SetString("PlayerSprites", _playerSprites[(i + 1) % 4].name); //save data
                return; // in order to avoid infenity loop
            }
        }
    }

    public void SetFullscreen()
    {
        if (Screen.fullScreen)
            Screen.SetResolution(_minWidth, _minHeight, !Screen.fullScreen);
        else
            Screen.SetResolution(_resolutions[_resolutions.Length - 1].width, _resolutions[_resolutions.Length - 1].height, !Screen.fullScreen);

        _FullScreenText.SetText((!Screen.fullScreen).ToString());
    }

    public void StartSingleplayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartMultiplayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #region Dynamic Function
    /// <summary>
    /// Update the music volume by player choice
    /// </summary>
    /// <param name="volume">New volume to the mixer</param>
    public void SetMusicVolume(float volume)
    {
        _MusicMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    /// <summary>
    /// Update the music volume by player choice
    /// </summary>
    /// <param name="volume">New volume to the mixer</param>
    public void SetSoundVolume(float volume)
    {
        _SoundMixer.SetFloat("SoundVolume", volume);
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }
    #endregion
}