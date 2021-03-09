using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ScoreHandlerMulti : ScoreHandler
{

    // Use this for initialization
    void Start()
    {
        while (!GameObject.Find("Player(Clone)").GetActive()) { }

        _inGameCombo        = _inGameComboGO.GetComponent<TextMeshProUGUI>();
        _comboMessegeText   = _comboMessegeGO.GetComponent<TextMeshProUGUI>();
        _comboMessege       = _comboMessegeGO.GetComponent<Transform>();
        _rb2d               = GameObject.Find("Player(Clone)").GetComponent<Rigidbody2D>();
        _timeLeft2Combo = _maxTime;
        _timerBar.fillAmount = 0;
    }

    private void Update()
    {
        if (_currCombo != 0)
        {
            if (_timeLeft2Combo > 0)
            {
                _timeLeft2Combo -= Time.deltaTime;
                _timerBar.fillAmount = _timeLeft2Combo / _maxTime;
            }

            if (_timeLeft2Combo <= 0)
                // we have a problem in multiplayer houston
                CalculateCombo(_rb2d);
        }
    }
}