using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public List<SpawnerData> spawners = new List<SpawnerData>();

    [SerializeField] private Transform entitiesTransform;

    public void AddSpawner(RoadieComponent.TileRow tileRow, float spawnHeight)
    {
        spawners.Add(new SpawnerData(tileRow, spawnHeight));
    }

    public void Update()
    {

        for (int i = 0; i < spawners.Count; i++)
        {
            SpawnerData spawner = spawners[i];

            if (spawner.timeToNextSpawn <= 0)
            {
                spawner.timeToNextSpawn += Random.Range(spawner.tileRow.minSpawnPeriod, spawner.tileRow.maxSpawnPeriod);
                Vector3 spawnPos;
                if (spawner.tileRow.facingRight)
                {
                    spawnPos = new Vector3(-(GameManager.TilemapWidth / 2f) - spawner.tileRow.spawnPosXOffset, spawner.spawnHeight);
                }
                else
                {
                    spawnPos = new Vector3((GameManager.TilemapWidth / 2f) + spawner.tileRow.spawnPosXOffset, spawner.spawnHeight);
                }
                GameObject entity = Instantiate(spawner.tileRow.prefabCycle[spawner.nextPrefabIndex], spawnPos, Quaternion.identity, entitiesTransform);
                VehicleHandler vehicleHandler = entity.GetComponent<VehicleHandler>();
                if (spawner.tileRow.facingRight)
                {
                    vehicleHandler.SetVelocityX(spawner.tileRow.speed);
                    //vehicleHandler.SetRotationZ(0);
                }
                else
                {
                    vehicleHandler.SetVelocityX(-spawner.tileRow.speed);
                    vehicleHandler.SetRotationZ(180);
                }
                spawner.nextPrefabIndex++;
                if (spawner.nextPrefabIndex == spawner.tileRow.prefabCycle.Count) spawner.nextPrefabIndex = 0;
            }
            else
            {
                spawner.timeToNextSpawn -= Time.deltaTime;
            }
        }
    }
}
