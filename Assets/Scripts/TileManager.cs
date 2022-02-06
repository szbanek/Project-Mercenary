using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public GameObject HexTileDirt;
    public SpriteRenderer spriteRenderer;
	public Sprite[] sprites;
    public Tilemap map;
    private int width = 10;
    private int height = 25;
    private float x_offset = 1.5f;
    private float y_offset = 0.435f;
    
    [SerializeField] private TileBase HexCrater;
    [SerializeField] private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;
    void Start()
    {
        //GenerateMap();
    }
    
    void GenerateMap()
    {
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                GameObject obj;
                obj = Instantiate(HexTileDirt);
                if (y % 2 == 0)
                {
                    obj.transform.position = new Vector3(x * x_offset, y * y_offset, 0);
                }
                else
                {
                    obj.transform.position = new Vector3(x * x_offset + x_offset/2, y * y_offset, 0);
                }

                if (x == 5)
                {
                    map.SetTile(new Vector3Int(x,y, 0), HexCrater);
                }
                setInfo(obj, x, y);
            }
        }
        
    }

    void setInfo(GameObject obj, int x, int y)
    {
        obj.transform.parent = transform;
        obj.name = x.ToString() + " " + y.ToString();
    }

    private void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("clicked");
            Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = map.WorldToCell(mPos);
            TileBase tile = map.GetTile(gridPos);
            bool walkable = dataFromTiles[tile].walkable;
            //Debug.Log("Clicked at: " + ToCube(gridPos) + "; walkable: " + walkable);
        }
        
    }

    public bool isWalkable(Vector2 pos)
    {
        Vector3Int gridPos = map.WorldToCell(pos);
        TileBase tile = map.GetTile(gridPos);
        if (tile == null)
        {
            return false;
        }
        else
        {
            return dataFromTiles[tile].walkable;
        }
    }

    public int getMovementModifier(Vector3Int gridPos)
    {
        TileBase tile = map.GetTile(gridPos);
        return dataFromTiles[tile].movementModifier;
    }
    
    public Vector3 ToCube(Vector3Int gridPos)
    {
        float q = gridPos.y;
        float r = gridPos.x - (gridPos.y - (Mathf.Abs(gridPos.y) %2))/2f;
        return new Vector3(q, r, -q-r);
    }

    public Vector3 ToPix(Vector3Int gridPos)
    {
        float size = x_offset/3f;
        float x = size*3/2*gridPos.y;
        float y = size * Mathf.Sqrt(3) * (gridPos.x + 0.5f * (Mathf.Abs(gridPos.y) %2));
        return new Vector3(x, y, 0);
    }
    public Vector3 getPosition(Vector2 pos)
    {
        Vector3Int gridPos = map.WorldToCell(pos);
        TileBase tile = map.GetTile(gridPos);
        if (tile == null)
        {
            return new Vector3(0,0,0);
        }
        else
        {
            return ToPix(gridPos);
        }
    }

    public Vector3Int getPositionGrid(Vector3 pos)
    {
        Vector2 p = new Vector2(pos.x, pos.y);
        return map.WorldToCell(pos);
    }

    Vector3 subtractCube(Vector3 A, Vector3 B)
    {
        return new Vector3(A.x - B.x, A.y - B.y, A.z - B.z);
    }

    public int getDistance(Vector3Int A, Vector3Int B)
    {
        Vector3 diff = subtractCube(ToCube(A), ToCube(B));
        return (int) Mathf.Max(Mathf.Abs(diff.x), Mathf.Max(Mathf.Abs(diff.y), Mathf.Abs(diff.z)));
    }
}