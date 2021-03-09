using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// this class veriables, Update(), InitiateFloors() is same as InstantiateFloors,
/// so there is change only with the way of showing serial number(10, 20, 30...)
/// </summary>
public class InstantiateFloorsMulti : InstantiateFloors
{
    public static int FloorPoolSize = 500;                                  // How many floors to keep on standby.
    public Canvas _canvas;
    public TMP_FontAsset _AntonSDF;
    private GameObject[] _serialNumbers;

    private const int WHITE = 255;

    /// <summary>
    /// create GameObject for serial numbers and and 'TextMeshProUGUI' Component to them
    /// the other basic Components of UI object (RectTransform and CanvasRenderer) are create auto
    /// we aslo instantiate the properties of the TextMeshProUGUI
    /// </summary>
    private void OnEnable()
    {
        // create the _serialNumbers GameObject
        _serialNumbers = new GameObject[FloorPoolSize / 10];
        for (int i = 0; i < _serialNumbers.Length; ++i)
        {
            _serialNumbers[i] = new GameObject("serialNumber" + i.ToString(), typeof(TextMeshProUGUI));
            _serialNumbers[i].transform.SetParent(_canvas.transform, false);
            TextMeshProUGUI text = _serialNumbers[i].GetComponent<TextMeshProUGUI>();
            // add the properties
            text.color = new Color(WHITE, WHITE, WHITE);
            text.font = _AntonSDF;
            text.fontSize = 25f;
            text.alignment = TextAlignmentOptions.Center;
            text.text = "";
            _serialNumbers[i].SetActive(true);
        }

        //Initialize the floors collection.
        _floors = new GameObject[FloorPoolSize];
        InitiateFloors();
    }

    private void Update()
    {
        for (int i = 10; i < FloorPoolSize; i += 10)
        {
            float floor = _floors[i].transform.position.y + _to10;
            if (floor % 20 == 0 && floor != 0)
            {
                Vector3 namePos = Camera.main.WorldToScreenPoint(_floors[i].transform.position);
                _serialNumbers[(i - 1) / 10].transform.position = namePos;
                _serialNumbers[(i - 1) / 10].GetComponent<TextMeshProUGUI>().text = ((int)floor / 2).ToString();
                _serialNumbers[(i - 1) / 10].SetActive(true);
            }
        }
    }

    public override void InitiateFloors()
    {
        float x = 0;
        float y = 0.15f;
        float floorSize = 0;

        //Loop through the collection and create the individual floors.
        for (int i = 0; i < FloorPoolSize; i++)
        {
            _floors[i] = PhotonNetwork.Instantiate(_floorPrefab.name, _floorPosition, Quaternion.identity, 0);

            if (i == 0 || i % 50 == 0)
            {
                _floors[i].transform.localScale = new Vector3(_maxXScale, 1, 1);
                _floors[i].transform.position = new Vector3(0, y);
            }
            else
            {
                floorSize = Random.Range(2.5f, 5);
                if (y > 300 && floorSize > 3) //player above floor 150
                    floorSize = 3f;
                else if (y > 500 && floorSize > 2.5f)//player above floor 250
                    floorSize = 2.5f;
                x = Random.Range(-(6 - 0.68f * floorSize), 6 - 0.68f * floorSize); // set range of distance according to size by 
                _floors[i].transform.position = new Vector3(x, y);
                _floors[i].transform.localScale = new Vector3(floorSize, 1, 1);
            }
            y += 2;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < FloorPoolSize; i++)
            Destroy(_floors[i]);
    }
}

