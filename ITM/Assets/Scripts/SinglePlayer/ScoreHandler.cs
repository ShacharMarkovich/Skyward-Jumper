using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ScoreHandler : MonoBehaviour
{
    public      GameObject      _inGameComboGO;               // the nuber we show in screen's left side - "xxx floors"
    protected   TextMeshProUGUI _inGameCombo;
    public      GameObject      _comboMessegeGO;              // wow, amazing, nice etc...
    protected   TextMeshProUGUI _comboMessegeText;
    protected   Transform       _comboMessege;
    public      TextMeshProUGUI _inGameScoreText;
    public      TextMeshProUGUI _endScoreText;
    public      TextMeshProUGUI _endFloorNoText;
    public      TextMeshProUGUI _endBestComboText;

    //for calculate time left for combo to end
    public    Image       _timerBar;
    protected const float _maxTime = 2f;
    protected float       _timeLeft2Combo;
    protected Rigidbody2D _rb2d;              // player's Rigidbody2D

    public AudioSource[] _combosAudio;

    protected const int _minCombo = 5;
    protected List<int> _combos = new List<int>();  // list of combos
    protected int _prevFloorNum = 0;                // last floor's number the player step on
    protected int _floorNum = 0;                    // current floor's number
    protected int _currCombo = 0;
    protected int _score = 0;

    // Use this for initialization
    void Start()
    {
        _comboMessegeText = _comboMessegeGO.GetComponent<TextMeshProUGUI>();
        _comboMessege     = _comboMessegeGO.GetComponent<Transform>();
        _inGameCombo      = _inGameComboGO.GetComponent<TextMeshProUGUI>();
        _rb2d             = GameObject.Find("Player").GetComponent<Rigidbody2D>();
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
            {
                // we have a problem in multiplayer houston
                CalculateCombo(_rb2d);
            }
        }
    }

    /// TODO: fix bug - combo finish not in right time
    public void CalculateCombo(Rigidbody2D rb2d)
    {
        _prevFloorNum = _floorNum;
        _floorNum = (int)rb2d.transform.position.y / 2;
        _endFloorNoText.text = "Floor: " + _floorNum;
        _score = CalculateScore();
        _inGameScoreText.text = "Score: " + _score.ToString();
        _endScoreText.text = "Score: " + _score.ToString();
        _endBestComboText.text = "Best Combo: " + CheckBestCombo().ToString();
        _inGameCombo.text = _currCombo.ToString();
        // only if there is combo, we show the text..
        if (_currCombo != 0)
            _inGameComboGO.SetActive(true);
        else
            _inGameComboGO.SetActive(false);
    }

    public int CalculateScore()
    {
        int score = 10 * _floorNum;

        UpdateCombo();

        for (int i = 0; i < _combos.Count; ++i)
            score += _combos[i] * _combos[i];

        return score;
    }

    private void UpdateCombo()
    {
        // cheack if player did a big jump

        if (_floorNum - _prevFloorNum > 1 && _timeLeft2Combo != 0)
        {
            _currCombo += _floorNum - _prevFloorNum;
            _timeLeft2Combo = _maxTime;
        }

        // factors that make player to end dig jump:
        else if (( _floorNum - _prevFloorNum == 1 || _floorNum - _prevFloorNum < 0 || _timeLeft2Combo <= 0) && _currCombo > 0)
        {
            if (_currCombo >= _minCombo) // below 5 is not considerd
            {
                _combos.Add(_currCombo);

                //show combo sign and sound
                if (_currCombo <= 7)
                {
                    _comboMessegeText.text = "good";
                    _comboMessegeText.fontSize = 60f;
                    _combosAudio[0].Play();
                }
                else if (_currCombo > 7 && _currCombo <= 14)
                {
                    _comboMessegeText.text = "sweet";
                    _comboMessegeText.fontSize = 60f;
                    _combosAudio[1].Play();
                }
                else if (_currCombo > 14 && _currCombo <= 24)
                {
                    _comboMessegeText.text = "nice";
                    _comboMessegeText.fontSize = 60f;
                    _combosAudio[2].Play();
                }
                else if (_currCombo > 24 && _currCombo <= 34)
                {
                    _comboMessegeText.text = "wow";
                    _comboMessegeText.fontSize = 60f;
                    _combosAudio[3].Play();
                }
                else if (_currCombo > 34 && _currCombo <= 49)
                {
                    _comboMessegeText.text = "super";
                    _comboMessegeText.fontSize = 60f;
                    _combosAudio[4].Play();
                }
                else if (_currCombo > 49 && _currCombo <= 74)
                {
                    _comboMessegeText.text = "amazing";
                    _comboMessegeText.fontSize = 75f;
                    _combosAudio[5].Play();
                }
                else if (_currCombo > 74 && _currCombo <= 99)
                {
                    _comboMessegeText.text = "extreme";
                    _comboMessegeText.fontSize = 75f;
                    _combosAudio[6].Play();
                }
                else if (_currCombo > 99 && _currCombo <= 149)
                {
                    _comboMessegeText.text = "fantastic";
                    _combosAudio[7].Play();
                }
                else if (_currCombo > 150 && _currCombo <= 199)
                {
                    _comboMessegeText.text = "unbeliveble";
                    _comboMessegeText.fontSize = 90f;
                    _combosAudio[8].Play();
                }
                else if (_currCombo >= 200)
                {
                    _comboMessegeText.text = "Like a Boss";
                    _comboMessegeText.fontSize = 100f;
                    _combosAudio[9].Play();
                }

                _comboMessegeGO.SetActive(false);
                _comboMessegeGO.SetActive(true);
            }

            _timerBar.fillAmount = 0; //make the bar empty
            _timeLeft2Combo = _maxTime; //for now, not sure whether it should be here...
            _currCombo = 0;
        }
    }

    public int CheckBestCombo()
    {
        if (_combos.Count > 0)
            return _combos.Max();
        else
            return 0;
    }
}