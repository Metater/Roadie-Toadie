using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tile Database", menuName = "Tile Database")]
public class TileDatabase : ScriptableObject
{
    [SerializeField] public List<TileBase> tiles = new List<TileBase>();

    public TileBase GetTile(string tileName)
    {
        return tiles.Find((tile) => tile.name == tileName);
    }

    public bool ContainsTile(string tileName)
    {
        foreach (TileBase tile in tiles)
            if (tile.name == tileName) return true;
        return false;
    }
}
