using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstantiateFloors : MonoBehaviour
{
    public    GameObject _floorPrefab;                        // The floor game object.
    private   int _floorPoolSize = 5;                         // How many floors to keep on standby.
    protected GameObject[] _floors;                           // Collection of pooled floors.
    protected Vector2 _floorPosition = new Vector2(0, 0.15f); // A holding position for our unused floors offscreen.

    protected const float _to10 = -0.15f;
    protected const float _maxXScale = 9f;

    private TextMeshProUGUI _serialNumber;
    
    private void Start()
    {
        _serialNumber = GameObject.Find("serialNumber").GetComponent<TextMeshProUGUI>();

        //Initialize the floors collection.
        _floors = new GameObject[_floorPoolSize];
        InitiateFloors();
    }

    void Update()
    {
        // if the first floor is 10X we show the floor number.
        float floor = _floors[0].transform.position.y + _to10;
        if (floor % 20 == 0 && floor != 0)
        {
            Vector3 namePos = Camera.main.WorldToScreenPoint(_floors[0].transform.position);
            _serialNumber.transform.position = namePos;
            _serialNumber.text = ((int)floor / 2).ToString();
        }
        else
            _serialNumber.text = "";
    }

    public virtual void InitiateFloors()
    {
        float x = 0;
        float y = 0.15f;
        float floorSize = 0;

        //Loop through the collection and create the individual floors.
        for (int i = 0; i < _floorPoolSize; i++)
        {
            _floors[i] = (GameObject)Instantiate(_floorPrefab, _floorPosition, Quaternion.identity); // create the floor itself

            if (i == 0)
            {
                _floors[i].transform.localScale = new Vector3(_maxXScale, 1, 1);
                _floors[i].transform.position = new Vector3(x, y);
            }
            else
            {
                floorSize = Random.Range(2.5f, 4);
                x = Random.Range(-(6 - 0.68f * floorSize), 6 - 0.68f * floorSize); // set range of distance according to size by 
                _floors[i].transform.position = new Vector3(x, y);
                _floors[i].transform.localScale = new Vector3(floorSize, 1, 1);
            }
            y += 2;
        }
    }
}