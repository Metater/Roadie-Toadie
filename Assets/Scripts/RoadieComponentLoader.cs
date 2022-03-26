using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadieComponentLoader : MonoBehaviour
{
    [SerializeField] private List<RoadieComponent> roadieComponents = new List<RoadieComponent>();
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TileDatabase tileDatabase;
    [SerializeField] private SpawnerManager spawnerManager;

    private Dictionary<string, TileBase> tileCache = new Dictionary<string, TileBase>();
    private int heightOffset = 0;

    private void Start()
    {
        foreach (RoadieComponent roadieComponent in roadieComponents)
        {
            int height = roadieComponent.tiles.Count;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < GameManager.TilemapWidth; x++)
                {
                    string tileName = roadieComponent.tiles[y].tileRow[x];
                    if (!tileCache.TryGetValue(tileName, out TileBase tile))
                    {
                        //tile = AssetDatabase.LoadAssetAtPath<TileBase>($"Assets/Tiles/{roadieComponent.tiles[y].tileRow[x]}.asset");
                        tile = tileDatabase.GetTile(roadieComponent.tiles[y].tileRow[x]);
                        tileCache.Add(tileName, tile);
                    }

                    tilemap.SetTile(new Vector3Int(x - GameManager.TilemapWidth / 2, heightOffset + y, 0), tile);
                }
                if (roadieComponent.tiles[y].prefabCycle.Count > 0) spawnerManager.AddSpawner(roadieComponent.tiles[y], heightOffset + y);
            }
            heightOffset += height;
        }

        playerManager.boundX.x = -(GameManager.TilemapWidth + 1) / 2;
        playerManager.boundX.y = (GameManager.TilemapWidth + 1) / 2;
        playerManager.boundY.x = -1;
        playerManager.boundY.y = heightOffset;
    }
}
