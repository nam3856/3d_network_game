using UnityEngine;

// 스크립트
public class MinimapCamera : MonoBehaviour
{
    public Camera Cam;
    private Transform _player;

    public void SetPlayerTransform(Transform player)
    {
        _player = player;
    }


    private void Update()
    {
        if (_player != null)
        {
            var pos = _player.position;
            pos.y = 10;
            transform.position = pos;
            
        }
    }
}
