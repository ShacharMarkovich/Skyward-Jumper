using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class cameraRoll : MonoBehaviour
{
    #region Class Variables

    public  Transform _player;
    private Transform _camera;
    
    public  GameObject      _hurryUp;               // Hurry up UI massage
    public  TextMeshProUGUI _countdownText;         // time left UI text
    private const int       _maxClockChallenge = 5; // max times for roll spped to speed up
    private int   _ccPast;                          // number of past Clock Challenges
    private int   _time;                            // time of each "full round of clock"
    private int   _timeLeft;                        // time left to rebegin The re-count

    private const float _speed = 0.02F;             // Each Clock Challenge that past, the camera roll in 0.02 faster
    private float       _rollSpeed;                 // Current roll speed

    private const float _minHeight = 8.15F;         // minimum height that the camera will move (4th floor)
    private bool        _callMoreThanOne = false;   // flag that check if the player has already been on floor 4

    private const float _backgroundHeight = 9.8f;
    private Vector3     _velocity;
    private Vector3     _targetPosition;
    #endregion 

    //initialization function
    void Start()
    {
        _camera = GetComponent<Transform>();
        _velocity = Vector3.zero;
        _rollSpeed = _speed;

        _ccPast = 0;
        _time = 30;
        _timeLeft = _time;
        _countdownText.text = _timeLeft.ToString();
    }

    void Update()
    {
        if (Time.timeScale == 1f) // this is for the PauseMenu - so when the time is 0, the camera won't move.
        {
            //min height to start move the camera
            if (_player.position.y > _minHeight || _callMoreThanOne)
            {
                if (!_callMoreThanOne)
                    StartCoroutine("LoseTime");

                _callMoreThanOne = true;

                UpdateRollSpeed();

                _camera.position = new Vector3(_camera.position.x, _camera.position.y + _rollSpeed, _camera.position.z);
            }

            // when the player is in the highest quarter of the screen, the camera follows him smoothly,
            // in order to avoid the player to get out from screen's top 
            if (_player.position.y > _camera.position.y + _backgroundHeight / 4)
            {
                _targetPosition = new Vector3(0, _player.position.y, -10);
                _camera.position = Vector3.SmoothDamp(_camera.position, _targetPosition, ref _velocity, 0.3f);
            }
        }
    }

    private void UpdateRollSpeed()
    {
        _countdownText.text = _timeLeft.ToString();

        // Hurry Up massage appear only for ~3 sec
        if (_timeLeft <= _time * 0.9)
            _hurryUp.SetActive(false);

        if (_timeLeft <= 0 && _ccPast != _maxClockChallenge) // time to speed up!
        {
            _ccPast++;

            _rollSpeed += _speed;
            _hurryUp.SetActive(true); // massage..
            StopCoroutine("LoseTime");

            //rebeginning time count down 
            _timeLeft = _time;
            StartCoroutine("LoseTime");
        }

        //Temporary, until we do a broken clock's animation. Just to see that all 5 clock challenge have been past
        if (_ccPast == _maxClockChallenge)
            _countdownText.text = "Broken";

    }

    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            _timeLeft--;
        }
    }
}