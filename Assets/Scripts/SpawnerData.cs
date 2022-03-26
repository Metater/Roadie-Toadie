using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerData
{
    public RoadieComponent.TileRow tileRow;
    public float spawnHeight;

    public float timeToNextSpawn = 0;
    public int nextPrefabIndex = 0;

    public SpawnerData(RoadieComponent.TileRow tileRow, float spawnHeight)
    {
        this.tileRow = tileRow;
        this.spawnHeight = spawnHeight;
    }
}
