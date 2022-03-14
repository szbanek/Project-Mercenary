using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassManager : MonoBehaviour {
    [SerializeField] private Image _arrow;
    private List<Vector3Int> _hubsToVisit;
    private PlayerMovement _player;
    private TileManager _map;
    private RectTransform _RT;
    
    void Awake() {
        _hubsToVisit = new List<Vector3Int> ();
        _player = FindObjectOfType<PlayerMovement> ();
        _map = FindObjectOfType<TileManager> ();
        _RT = _arrow.GetComponent<RectTransform>();
        GameManager.PlayerTurnMain += GameManagerOnPTM;
    }

    void OnDestroy() {
        GameManager.PlayerTurnMain -= GameManagerOnPTM;
    }

    void GameManagerOnPTM()
    {
        float dir = CalculateDirection ();
        _RT.rotation = Quaternion.Euler(new Vector3(0, 0, -dir));
    }

    private float CalculateDirection() {
        var playerPos = _player.GetPosGrid ();
        var distMin = int.MaxValue;
        var choice = new Vector3Int ();
        foreach (var hub in _hubsToVisit)
        {
            var curDist = _map.getDistance (playerPos, hub);
            if (curDist < distMin)
            {
                choice = hub;
                distMin = curDist;
            }
        }
        Vector3 targ = _player.GetPosCoor ();
        targ.z = 0f;
        targ.x = choice.x - targ.x;
        targ.y = choice.y - targ.y;
        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;

        return angle;
    }

    public void SetHubs(List<Vector3Int> hubs) {
        _hubsToVisit = hubs;
    }

    public void DeleteHub(Vector3Int hub) {
        _hubsToVisit.Remove (hub);
    }
}
