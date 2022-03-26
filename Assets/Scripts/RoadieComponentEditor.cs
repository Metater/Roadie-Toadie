using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadieComponentEditor : MonoBehaviour
{
    [SerializeField] private TileDatabase tileDatabase;
    [SerializeField] private Tilemap tilemap;
    [Space]
    [SerializeField] private string roadieComponentName = "";
    [SerializeField] private int height = 0;
    [SerializeField] private bool save = false;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (save)
        {
            save = false;
            if (roadieComponentName == "")
            {
                Debug.Log("Cannot save unnamed component!");
                return;
            }

            RoadieComponent roadieComponent = ScriptableObject.CreateInstance<RoadieComponent>();
            roadieComponent.name = roadieComponentName;
            for (int y = 0; y < height; y++)
            {
                string[] tileRow = new string[GameManager.TilemapWidth];
                for (int x = 0; x < GameManager.TilemapWidth; x++)
                {
                    TileBase tile = tilemap.GetTile(new Vector3Int(x - 9, y, 0));
                    tileRow[x] = tile.name;
                    if (!tileDatabase.ContainsTile(tile.name))
                        tileDatabase.tiles.Add(tile);
                }
                roadieComponent.tiles.Add(new RoadieComponent.TileRow(tileRow));
            }

            AssetDatabase.DeleteAsset($"Assets/Roadie Components/{roadieComponentName}.asset");
            AssetDatabase.CreateAsset(roadieComponent, $"Assets/Roadie Components/{roadieComponentName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


            Debug.Log("Saved!");

        }
    }
    #endif
}
