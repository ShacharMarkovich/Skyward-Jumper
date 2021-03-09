using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Class Variables
    private AudioSource[] _jumpsSound;

    // for animation
    private bool _playerFacingRight; // true = right, left = false
    private Animator _playerAnim;

    //player height = 5.8f
    private InGameMenus _InGameMenus;
    private ScoreHandler _scoreHandle;
    private Transform _camera;
    private Transform _player;
    private Rigidbody2D _rb2d;

    public float _speed;
    public float _jumpVelocity;
    private float _currentVelocity;
    private Vector2 _movement;
    private SyncData _syncData;

    private const float _maxVelocity = 20f;
    private const float _drag = 0.8f;
    private bool _grounded = true;
    public bool Grounded
    {
        get { return _grounded; }
    }
    #endregion

    void Awake()
    {
        _InGameMenus = GameObject.FindObjectOfType<InGameMenus>();
        if (SceneManager.GetActiveScene().buildIndex == 2)
            _syncData = GetComponent<SyncData>();
    }

    void Start()
    {
        _camera = GameObject.Find("Camera").GetComponent<Transform>();
        _player = GetComponent<Transform>();
        _rb2d = GetComponent<Rigidbody2D>();
        _playerAnim = GetComponent<Animator>();
        _playerAnim.runtimeAnimatorController = Instantiate(Resources.Load(PlayerPrefs.GetString("PlayerSprites", "Robot"))) as RuntimeAnimatorController;

        Time.timeScale = 1f;
        _playerFacingRight = true;

        _scoreHandle = GameObject.FindObjectOfType<ScoreHandler>();
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            _jumpsSound = new AudioSource[(int)Sounds.JumpSounds];
            for (int i = 0; i < (int)Sounds.JumpSounds; i++)
            {
                string name = (i + 1).ToString() + "FloorJumpSound";
                _jumpsSound[i] = GameObject.Find(name).GetComponent<AudioSource>();
            }
        }
    }

    void Update()
    {
        _currentVelocity = _rb2d.velocity.x;
        _movement = new Vector2(Input.GetAxis("Horizontal"), 0);

        //if player moving - change to "Run" animation
        _playerAnim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

        Flip();

        if (SceneManager.GetActiveScene().buildIndex == 2)
            if (_player.position.y / 2 >= 500)
                _InGameMenus.ShowGameOverMenu(true);
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        _currentVelocity = _rb2d.velocity.x;

        if (!(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            && _currentVelocity != 0 && _grounded)
            _rb2d.velocity = new Vector2(_drag * _currentVelocity, _rb2d.velocity.y);

        if (_currentVelocity * horizontal > 0)
            _rb2d.AddForce(_movement * _speed);
        else
        {
            if (_grounded)
                _rb2d.AddForce(8 * _movement * _speed);
            else
                _rb2d.AddForce(2 * _movement * _speed);
        }
        Jump();

        //if player moving - change to "Run" animation
        _playerAnim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        Flip();
    }


    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor") // check only collision from the top
        {
            if (other.relativeVelocity.y >= 0f)
            {
                _scoreHandle.CalculateCombo(_rb2d);

                _grounded = true;

                // we are on the ground - change to fit animation layer
                _playerAnim.SetLayerWeight(1, 0);

                // unplay "Jump" animation and start play "Land" animation
                _playerAnim.ResetTrigger("Jump");
                _playerAnim.SetBool("Land", false);
            }
        }
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor")
        {
            _grounded = false;

            // the player isn't on the ground - change to fit animation layer
            _playerAnim.SetLayerWeight(1, 1);
            _playerAnim.SetTrigger("Jump");

            // play "Jump" animation
            _playerAnim.SetTrigger("Jump");
        }
    }

    void OnBecameInvisible()
    {
 
        //TODO: in multyplayer there is situation that the floor is invisible,
        //      but the plyer still can stand in it without losing.
        //      ew need to check the high:
        //      if (cam.high - plyer.high < 4.2) _InGameMenus.ShowGameOverMenu();
        // show game over menu
        if (_player.position.y < _camera.position.y)
        {
            if (SceneManager.GetActiveScene().buildIndex == 2 && !_InGameMenus.IsGameOver())
            {
                _syncData._amIAlive = false;
                _InGameMenus.ShowGameOverMenu(false);
            }
            else
                _InGameMenus.ShowGameOverMenu();
        }
    }

    // contain implementation of jumps (including big ones)
    void Jump()
    {
        if (Input.GetKey("space") && _grounded && _rb2d.velocity.y == 0)
        {
            if (_currentVelocity > -_maxVelocity * 1 / 5 && _currentVelocity < _maxVelocity * 1 / 5)
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, _jumpVelocity);
                _jumpsSound[(int)Sounds.Jump1FloorSound].Play();
            }
            /// CR: make it with loop
            else if ((_currentVelocity > -_maxVelocity * 2 / 5 && _currentVelocity < -_maxVelocity * 1 / 5)
                || (_currentVelocity < _maxVelocity * 2 / 5 && _currentVelocity > _maxVelocity * 1 / 5))
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, _jumpVelocity * Mathf.Sqrt(2));
                _jumpsSound[(int)Sounds.Jump2FloorSound].Play();
            }
            else if ((_currentVelocity > -_maxVelocity * 3 / 5 && _currentVelocity < -_maxVelocity * 2 / 5)
                || (_currentVelocity < _maxVelocity * 3 / 5 && _currentVelocity > _maxVelocity * 2 / 5))
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, _jumpVelocity * Mathf.Sqrt(3));
                _jumpsSound[(int)Sounds.Jump3FloorSound].Play();
            }

            else if ((_currentVelocity > -_maxVelocity * 4 / 5 && _currentVelocity < -_maxVelocity * 3 / 5)
                || (_currentVelocity < _maxVelocity * 4 / 5 && _currentVelocity > _maxVelocity * 3 / 5))
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, _jumpVelocity * Mathf.Sqrt(4));
                _jumpsSound[(int)Sounds.Jump4FloorSound].Play();
            }

            else if ((_currentVelocity > -_maxVelocity && _currentVelocity < -_maxVelocity * 4 / 5)
                || (_currentVelocity < _maxVelocity && _currentVelocity > _maxVelocity * 4 / 5))
            {
                _rb2d.velocity = new Vector2(_rb2d.velocity.x, _jumpVelocity * Mathf.Sqrt(5));
                _jumpsSound[(int)Sounds.Jump5FloorSound].Play();
            }
        }
        // if  the player falling down - cange to "Land" animation
        else if (_rb2d.velocity.y < 0)
            _playerAnim.SetBool("Land", true);
    }

    /// this func change between the facing side of the player.
    /// used for animation only
    void Flip()
    {
        if (Input.GetAxis("Horizontal") > 0 && !_playerFacingRight || Input.GetAxis("Horizontal") < 0 && _playerFacingRight)
        {
            _playerFacingRight = !_playerFacingRight;
            if (_playerFacingRight)
                transform.eulerAngles = Vector3.zero;
            else
                transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}