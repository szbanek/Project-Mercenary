using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    public bool walkable = true;

    public int movementModifier = 0;
    public bool seeThrough = true;
    public bool visible = true; //true = character is visible

}

