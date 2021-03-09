using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SyncData : MonoBehaviour
{
    public  Transform       _playerTransform;
    public  bool            _amIAlive;
    public  bool            _areYouAlive;
    private TextMeshProUGUI _opponentFloor;
    private InGameMenus     _InGameMenusScript;
    private int             _currFloor = 0;

    // Use this for initialization
    void Start()
    {
        _amIAlive = true;
        _areYouAlive = true;

        PhotonNetwork.sendRate = 20;
        PhotonNetwork.sendRateOnSerialize = 10;
        _opponentFloor = GameObject.Find("OpponentFloor").GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        _InGameMenusScript = GameObject.FindObjectOfType<InGameMenus>();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext((int)_playerTransform.position.y / 2);
            stream.SendNext((bool)_amIAlive);
        }
        else
        {
            _currFloor = (int)stream.ReceiveNext();
            _opponentFloor.SetText(_currFloor.ToString());
            _areYouAlive = (bool)stream.ReceiveNext();
        }

        if (!_areYouAlive)
            _InGameMenusScript.ShowGameOverMenu(true);
        else if (_currFloor >= InstantiateFloorsMulti.FloorPoolSize)
            _InGameMenusScript.ShowGameOverMenu(false);
    }
}
