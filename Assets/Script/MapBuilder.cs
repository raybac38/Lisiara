using System;
using UnityEngine;

public class MapBuilder
{
    private float perlinNoiseMapScale = 0.015f;
    private float perlinNoiseStrenght = 25f;
    private float mapWaterLevel = 50;
    /// <summary>
    /// Generate a new map
    /// </summary>
    /// <returns>map data</returns>
    public bool[,,] Generate(Vector3Int mapSize)
    {
        bool[,,] mapData = new bool[mapSize.x, mapSize.y, mapSize.z];


        for (int x = 0; x < mapSize.x; x++) 
        { 
            for(int z = 0; z <mapSize.z; z++)
            {
                float groundLevel = Mathf.PerlinNoise(perlinNoiseMapScale * x, perlinNoiseMapScale * z);
                groundLevel = groundLevel * groundLevel * perlinNoiseStrenght + mapWaterLevel;

                for(int y = 0; y < groundLevel && y < mapSize.y; y++)
                {
                    mapData[x, y, z] = true;
                }
            }
        }
        return mapData;
    }
}
