using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileSwap {
    public int fromTileNum;
    public GameObject swapPrefab;
    public GameObject guaranteedDrop;
    public int toTileNum = 1; // Default 1 is ignored due to a Unity bug
}

public class TileSwapManager : MonoBehaviour {
    private static TileSwapManager S;
    private static Dictionary<int, TileSwap> TILE_SWAP_DICT;

    public List<TileSwap> tileSwapList;

    void Awake() {
        S = this;
    }

    public static void SWAP_TILES(int[,] map) {
        if (TILE_SWAP_DICT == null)
            S.BuildTileSwapDict();

        int fromTileNum;
        TileSwap tSwap;

        for (int i = 0; i < map.GetLength(0); i++) {
            for (int j = 0; j < map.GetLength(1); j++) {
                fromTileNum = map[i, j];

                if (TILE_SWAP_DICT.ContainsKey(fromTileNum)) {
                    tSwap = TILE_SWAP_DICT[fromTileNum];
                    map[i, j] = tSwap.toTileNum;

                    // Instantiate and Init the swapPrefab ISwappable
                    GameObject go = Instantiate<GameObject>(tSwap.swapPrefab); // Instantiate the swapPrefab

                    ISwappable iSwap = go.GetComponent<ISwappable>(); // Try to get the ISwappable component

                    if (iSwap != null) { // Check if the ISwappable component was found
                        iSwap.Init(fromTileNum, i, j); // Initialize the ISwappable with the tile number and positions
                        iSwap.guaranteedDrop = tSwap.guaranteedDrop; // Set the guaranteedDrop property
                    } else {
                        go.transform.position = new Vector3(i, j, 0) + MapInfo.OFFSET; // Set the position if ISwappable component is not found
                    } 
                }
            }
        }
    }

    void BuildTileSwapDict() {
        TILE_SWAP_DICT = new Dictionary<int, TileSwap>();

        foreach (TileSwap swap in tileSwapList) {
            if (TILE_SWAP_DICT.ContainsKey(swap.fromTileNum)) {
                Debug.LogError("More than one TileSwap with a From # of " + swap.fromTileNum);
            } else {
                TILE_SWAP_DICT.Add(swap.fromTileNum, swap);
            }
        }
    }
}

