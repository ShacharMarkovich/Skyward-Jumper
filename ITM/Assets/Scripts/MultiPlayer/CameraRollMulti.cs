using UnityEngine;

public class CameraRollMulti : MonoBehaviour {

    private GameObject _cam;
    private GameObject _player;

    private const float _backgroundHeight = 9.8f;
    private Vector3 _targetPosition;
    private Vector3 _velocity;

    void Start()
    {
        _player = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().PlayerGO;
        _cam = GameObject.Find("Camera");
        _velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player)
        {
            // when the player is in the highest quarter of the screen, the camera follows him smoothly
            if (_player.transform.position.y > _cam.transform.position.y + _backgroundHeight / 4)
            {
                _targetPosition = new Vector3(0, _player.transform.position.y, -10);
                _cam.transform.position = Vector3.SmoothDamp(_cam.transform.position, _targetPosition, ref _velocity, 0.3f);
            }
        }
    }
}