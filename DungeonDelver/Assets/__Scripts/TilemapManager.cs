using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour {
    static public Tile[] DELVER_TILES;
    static public Dictionary<char, Tile> COLL_TILE_DICT;

    [Header("Inscribed")]
    public Tilemap visualMap;
    public Tilemap collisionMap;
    public Tilemap grapTilesMap;

    private TileBase[] visualTileBaseArray;
    private TileBase[] collTileBaseArray;
    private TileBase[] grapTileBaseArray;

    void Awake() {
        LoadTiles();
    }

    void Start() {
        ShowTiles();
    }

    void LoadTiles() {
        int num;

        Tile[] tempTiles = Resources.LoadAll<Tile>("Tiles_Visual");

        DELVER_TILES = new Tile[tempTiles.Length];
        for (int i = 0; i < tempTiles.Length; i++) {
            string[] bits = tempTiles[i].name.Split('_');
            if (int.TryParse(bits[1], out num)) {
                DELVER_TILES[num] = tempTiles[i];
            } else {
                Debug.LogError("Failed to parse num of: " + tempTiles[i].name);
            }
        }
        Debug.Log("Parsed " + DELVER_TILES.Length + " tiles into TILES_VISUAL.");

        tempTiles = Resources.LoadAll<Tile>("Tiles_Collision");
        COLL_TILE_DICT = new Dictionary<char, Tile>();
        for (int i = 0; i < tempTiles.Length; i++)
        {
            char c = tempTiles[i].name[0];
            COLL_TILE_DICT.Add(c, tempTiles[i]);
        }
        Debug.Log("COLL_TILE_DICT contains " + COLL_TILE_DICT.Count + " tiles.");
    }

    void ShowTiles() {
    
        visualTileBaseArray = GetMapTiles();
        visualMap.SetTilesBlock(MapInfo.GET_MAP_BOUNDS(), visualTileBaseArray);

        collTileBaseArray = GetCollisionTiles();        
        collisionMap.SetTilesBlock(MapInfo.GET_MAP_BOUNDS(), collTileBaseArray);

        grapTileBaseArray = GetGrapTiles();       
        grapTilesMap.SetTilesBlock(MapInfo.GET_MAP_BOUNDS(), grapTileBaseArray);
    }

    public TileBase[] GetMapTiles() {
        int tileNum;
        Tile tile;
        TileBase[] mapTiles = new TileBase[MapInfo.W * MapInfo.H];
        for (int y = 0; y < MapInfo.H; y++) {
            for (int x = 0; x < MapInfo.W; x++) {
                tileNum = MapInfo.MAP[x, y];
                tile = DELVER_TILES[tileNum];
                mapTiles[y * MapInfo.W + x] = tile;
            }
        }
        return mapTiles;
    }

    public TileBase[] GetCollisionTiles()
    {
        Tile tile;
        int tileNum;
        char tileChar;
        TileBase[] mapTiles = new TileBase[MapInfo.W * MapInfo.H];
        for (int y = 0; y < MapInfo.H; y++)
        {
            for (int x = 0; x < MapInfo.W; x++)
            {
                tileNum = MapInfo.MAP[x, y];

                tileChar = MapInfo.COLLISIONS[tileNum];
                tile = COLL_TILE_DICT[tileChar];
                mapTiles[y * MapInfo.W + x] = tile;
            }
        }
        return mapTiles;
    }

    /// <summary>
    /// Use MapInfo.MAP and MapInfo.GRAP_TILES to create a TileBase[] array
    /// holding the tiles to fill the grapTilesMap Tilemap.
    /// </summary>
    /// <returns>The TileBases for grapTilesMap</returns>
    public TileBase[] GetGrapTiles() {
        Tile tile;
        int tileNum;
        char tileChar;
        TileBase[] mapTiles = new TileBase[MapInfo.W * MapInfo.H];
        for (int y = 0; y < MapInfo.H; y++) {
            for (int x = 0; x < MapInfo.W; x++) {
                tileNum = MapInfo.MAP[x, y];
                tileChar = MapInfo.GRAP_TILES[tileNum];
                if (tileChar == 'U') tileChar = '_';
                tile = COLL_TILE_DICT[tileChar];
                mapTiles[y * MapInfo.W + x] = tile;
            }
        }
        return mapTiles;
    }

}
