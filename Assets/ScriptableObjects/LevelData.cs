using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public List<TileData> tileDatas;
    public List<TileBase> tiles;
    public int[] distanceBetweenRooms = {25,50};
    [Range (2, 15)] public int numOfRooms = 5;
    public bool startRandomly = true;
    public int[] walkLength = {10, 20};
    public int[] iterations = {50, 150};
    [Range(0,100)]public int corridorPercentage = 20;
}
