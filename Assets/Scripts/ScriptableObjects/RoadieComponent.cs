using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Roadie Component", menuName = "Roadie Component")]
public class RoadieComponent : ScriptableObject
{
    [SerializeField] private bool refresh = false;

    [SerializeField] public List<TileRow> tiles = new List<TileRow>();

    private void OnValidate()
    {
        if (refresh)
        {
            refresh = false;

            foreach (TileRow tileRow in tiles)
            {
                foreach (string tile in tileRow.tileRow)
                    Debug.Log(tile);
            }
        }
    }

    [Serializable]
    public struct TileRow
    {
        public string[] tileRow;
        public List<GameObject> prefabCycle;
        public float spawnPosXOffset;
        public bool facingRight;
        public float speed;
        public float minSpawnPeriod;
        public float maxSpawnPeriod;

        public TileRow(string[] tileRow)
        {
            this.tileRow = tileRow;
            prefabCycle = new List<GameObject>();
            spawnPosXOffset = 0;
            facingRight = false;
            speed = 0;
            minSpawnPeriod = 0;
            maxSpawnPeriod = 0;
        }
    }

}
