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
            Debug.Log("clicked");
            Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = map.WorldToCell(mPos);
            TileBase tile = map.GetTile(gridPos);
            bool walkable = dataFromTiles[tile].walkable;
            Debug.Log("Clicked at: " + gridPos + "; walkable: " + walkable);
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
    
    Vector3 ToCube(Vector3Int gridPos)
    {
        float q = gridPos.x;
        float r = gridPos.y - (gridPos.x - (gridPos.x & 1));
        return new Vector3(q, r, -q - r);
    }

    Vector3 ToPix(Vector3Int gridPos)
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

}