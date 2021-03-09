using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public GameObject _pauseMenu;
    public GameObject _GameOverMenu;

    public AudioSource[] _AudioSources;

    // Update is called once per frame
    void Update()
    {
        if (_pauseMenu.GetActive() || _GameOverMenu.GetActive())
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                _AudioSources[(int)Sounds.SelectButtonSound].Play();
            else if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.W) ||
                 Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S))
                _AudioSources[(int)Sounds.MoveButtonSound].Play();
        }
    }
}

public enum Sounds
{
    SelectButtonSound = 0,
    MoveButtonSound = 1,
    BackgroundMusic = 2,
    // jumps sounds:
    Jump1FloorSound = 0,
    Jump2FloorSound = 1,
    Jump3FloorSound = 2,
    Jump4FloorSound = 3,
    Jump5FloorSound = 4,
    JumpSounds = 5,
    //MultiPlayer game over sounds:
    LoseSound = 0,
    WinSound = 1
}