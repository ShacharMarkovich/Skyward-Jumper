using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// this class called when there is 2 player connected to room.
/// after the count down end, the game start.
/// </summary>
public class Countdown : MonoBehaviour
{
    public TextMeshProUGUI _CountdownText;
    public GameObject _waitingScreen;

    private const float _max = 1f;  // time that every number is shown
    private float _alpha;           // CountdownText.color.a (0- Clear, 1- seen)
    private float _num;              // num of sec to count down
    private Color _color;           // CountdownText.color

    private void Start()
    {
        PlayerPrefs.SetInt("countdownIsEnd", 0);
        _color = _CountdownText.color;
        _num = 3;
        _alpha = 0;
        StartCoroutine("LoseTime");
    }

    private void Update()
    {
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            // _alpha <= 0 it's meen that a number is "finish" to be seen
            if (_alpha <= 0) // restart the coount down
            {
                StopCoroutine("LoseTime");
                _alpha = _max;

                if (_num != 0)
                    _CountdownText.text = _num.ToString();
                else if (_num == 0)
                    _CountdownText.text = "Go!";

                _num--;
                StartCoroutine("LoseTime");
            }
            _color.a = _alpha;
            _CountdownText.color = _color;

            if (_num < -1) //end of the count down
            {
                _CountdownText.text = "";
                PlayerPrefs.SetInt("countdownIsEnd", 1);
            }
        }
        else
        {
            _CountdownText.text = "";
            _waitingScreen.SetActive(true);
        }
    }

    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / (float)25.5);
            _alpha -= 1 / (float)25.5;

        }
    }
}