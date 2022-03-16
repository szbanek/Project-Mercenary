using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject _player;

    public TileManager _map;

    private void Awake() {
        GameManager.MapReady += GameManagerOnMapReady;
    }

    private void OnDestroy() {
        GameManager.MapReady -= GameManagerOnMapReady;
    }

    void GameManagerOnMapReady(int i) {
        Camera.main.transform.position = new Vector3 (0, 0, Camera.main.transform.position.z);
    }

    public void UpdateCamera()
    {
        var camPos = Camera.main.transform.position;
        var dir = new Vector2 ();
        var playerPos = _player.transform.position;
        dir = (Vector2)(camPos - playerPos);
        camPos = new Vector3 (camPos.x - dir.x, camPos.y - dir.y, camPos.z);
        Camera.main.transform.position = camPos;
    }
}
