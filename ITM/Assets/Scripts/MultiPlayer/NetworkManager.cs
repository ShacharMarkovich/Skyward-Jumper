using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// http://doc-api.photonengine.com/en/pun/current/index.html
public class NetworkManager : Photon.PunBehaviour
{
    #region Class Variables
    // This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
    private static string _gameVersion = "1";
    private static byte   _MAX = 2;

    public  GameObject _GM;                      // Game Manager GameObject
    public  GameObject _waitingScreen;
    public  GameObject _BackgroundMusic;
    public  GameObject _playerPrefab;
    private GameObject _myPlayerGO = null;
    public  GameObject PlayerGO
    {
        get { return _myPlayerGO; }
    }

    private bool _calledMoreThatOnce = false;
    private bool _autoConnect = true;

    #endregion


    #region MonoBehaviour CallBacks
    // MonoBehaviour method called on GameObject by Unity during early initialization phase.
    void Awake()
    {
        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
    }

    // MonoBehaviour method called on GameObject by Unity during initialization phase.
    void Start()
    {
        Time.timeScale = 1f; // in order to prevent bugs
        PlayerPrefs.SetInt("countdownIsEnd", 0);
        Connect();
    }

    void Update()
    {
        Countdown cd = _GM.GetComponent<Countdown>();
        if (PhotonNetwork.inRoom && PhotonNetwork.room.PlayerCount == _MAX && !_calledMoreThatOnce)
        {
            _waitingScreen.SetActive(false);
            cd.enabled = true;
            _BackgroundMusic.SetActive(true);
            if (PlayerPrefs.GetInt("countdownIsEnd", 0) == 1)
            {
                cd.enabled = false;
                _calledMoreThatOnce = true;
                PlayerPrefs.SetInt("countdownIsEnd", 0);
                _myPlayerGO.GetComponent<PlayerController>().enabled = true;
                _myPlayerGO.transform.position = new Vector3(_myPlayerGO.transform.position.x, 1f, _myPlayerGO.transform.position.z);
            }
        }
    }
    /*
    // show in the game connection's Detailed
    void OnGUI()
    {
        GUI.color = Color.black;
        GUI.skin.label.fontSize = 30;
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
    */
    #endregion


    #region Public Methods
    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected)
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        else
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
    }
    #endregion


    #region Photon.PunBehaviour CallBacks
    // called when connected to server
    // called only if PhotonNetwork.autoJoinLobby == false
    public override void OnConnectedToMaster()
    {
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
        if (_autoConnect)
        {
            PhotonNetwork.JoinRandomRoom();
            _autoConnect = false;
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        //TODO: if the leaving player is master - do what?
        //TODO: else - keep the room going
    }

    // Failed to join a random room, maybe none exists or they are all full -> Create a new room.
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = _MAX }, null);
    }

    // if the client is MasterClient - load floors
    // Spawn the Player
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.isMasterClient)
            _GM.GetComponent<InstantiateFloorsMulti>().enabled = true;
        SpawnMyPlayer();
        _GM.GetComponent<CameraRollMulti>().enabled = true;
        _GM.GetComponent<InGameMenus>().enabled = true;
        _GM.GetComponent<ScoreHandlerMulti>().enabled = true;
    }

    void SpawnMyPlayer()
    {
        Vector3 spawnPoint;
        if (PhotonNetwork.isMasterClient)
            spawnPoint = new Vector3(5f, 1f, 0f);
        else
            spawnPoint = new Vector3(-5f, 1f, 0f);

        _myPlayerGO = PhotonNetwork.Instantiate(this._playerPrefab.name, spawnPoint, Quaternion.identity, 0);

        Color color = _myPlayerGO.GetComponent<SpriteRenderer>().color;
        color.a = 255;
        _myPlayerGO.GetComponent<SpriteRenderer>().color = color;

        _myPlayerGO.GetComponent<BoxCollider2D>().enabled = true;
    }
    #endregion


    #region Photon Messages

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        //maybe here we can fix the unable to jump bug
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.room.IsVisible = false;
                PhotonNetwork.room.IsOpen = false;
            }
            PhotonNetwork.LeaveRoom();
        }
    }
    #endregion
}