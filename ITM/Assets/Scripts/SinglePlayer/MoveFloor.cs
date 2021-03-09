using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Update floor location and size when become invisible
/// </summary>
public class MoveFloor : MonoBehaviour
{
    private Transform _floor;
    private const float _xScale = 9f;
    private const float _to100 = -0.15f;

    /// TODO: make the game harder - make the floors further
    /// private float _lastPosition; // in order to make the disrance between floors bigger

    void Start()
    {
        _floor = gameObject.GetComponent<Transform>();
    }

    /// <summary>
    /// Update floor position and scale(when became invisible).
    /// Every 50 floors - a big one.
    /// Also consider size and position in order to make the game harder.
    /// </summary>
    void OnBecameInvisible()
    {
        // We'll use it later. Saves unnecessary turning to variables
        int y = (int)(_floor.position.y + _to100) + 10;

        //in order to make the game harder - make the floors smallers
        float floorSize = Random.Range(2.5f, 5);

        //player above floor 150
        if (y > 300 && floorSize > 3)
            floorSize = 3f;
        // player above floor 250
        else if (y > 500 && floorSize > 2.5f)
            floorSize = 2.5f;

        // set range of distance according to size by update floor position and scale
        float x = Random.Range(-(6 - 0.68f * floorSize), 6 - 0.68f * floorSize);

        _floor.position = new Vector3(x, _floor.position.y + 10f);
        _floor.localScale = new Vector3(floorSize, 1, 1);

        if (y % 2 != 0) //fix bug
            y++;
        // every 50 levels - one big floor (every 100.15 in high)
        if (y % 100 == 0)
        {
            _floor.position = new Vector3(0, _floor.position.y);
            _floor.localScale = new Vector3(_xScale, 1, 1);
        }
    }
}