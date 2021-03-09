using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRoll : MonoBehaviour
{
    public Transform _cam;

    public Transform _Background1;
    private Renderer _Bg1Re; // _Background1 Renderer
    public Transform _Background2;
    private Renderer _Bg2Re; // _Background2 Renderer

    private const float _height = 9.8f; // height of sprite

    void Start()
    {
        _Bg1Re = _Background1.GetComponent<Renderer>();
        _Bg2Re = _Background2.GetComponent<Renderer>();
    }

    void Update()
    {
        if (!_Bg1Re.isVisible && _Background1.position.y < _cam.position.y - _height / 2)
            _Background1.localPosition = new Vector2(_Background1.localPosition.x, _Background1.localPosition.y + 2 * _height);
        
        if (!_Bg2Re.isVisible && _Background2.position.y < _cam.position.y - _height / 2)
            _Background2.localPosition = new Vector2(_Background2.localPosition.x, _Background2.localPosition.y + 2 * _height);
    }
}
