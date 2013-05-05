using System.Collections;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using System.IO;
using System;
using UnityEngine;

public class MakeTerrain : MonoBehaviour
{
    private Noise2D noiseMap = null;
    public Terrain terrain;
    public bool regenerateTerrain = true;
    public int resolution = 1024;
    public double frequency = 1;
    public double lacunarity = 2;
    public double persistence = .5;
    public int octaves = 6;
    public int seed = 0;
    public enum QualitySetting
    {
        Low = QualityMode.Low,
        Medium = QualityMode.Medium,
        High = QualityMode.High
    };
    public QualitySetting qualitySetting = QualitySetting.Medium;
    public float zoom = 1f;
    public float offset = 0f;
    public bool seamless = true;
    public bool randomSeed = false;

    // Use this for initialization
    void Start ()
    {
        if (regenerateTerrain && terrain)
            GenerateTerrain ();
        else if (!terrain)
            Debug.LogError ("Terrain reference has not been set!");
    }
 
    /// <summary>
    /// Generates the terrain.
    /// </summary>
    void GenerateTerrain ()
    {
        if (randomSeed)
        {
            seed = (new System.Random ()).Next (1, 9999);
            Debug.Log ("Your randomly generated seed is: " + seed + ".");
        }
        // Create and set up a perlin generator with the given parameters.
        Perlin theGenerator = new Perlin (frequency, lacunarity, persistence, octaves, seed, (QualityMode) qualitySetting);
        noiseMap = new Noise2D (resolution, resolution, theGenerator);
        noiseMap.GeneratePlanar (
            offset + -1 * 1 / zoom, 
            offset + offset + 1 * 1 / zoom, 
            offset + -1 * 1 / zoom,
            offset + 1 * 1 / zoom,
         seamless);
        float[,] heightMap = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];
        for (int index = 0; index < terrain.terrainData.heightmapWidth; index++)
        {
            for (int index2 = 0; index2 < terrain.terrainData.heightmapWidth; index2++)
                heightMap[index, index2] = noiseMap[index, index2];
        }
        // Map the heightmap we created to the terrain data.
        terrain.terrainData.SetHeights (0, 0, heightMap);
        // Bubble up the changes to the terrain object.
        terrain.Flush ();
        // Jump for joy.
        float secondsTaken = Time.realtimeSinceStartup;
        if (secondsTaken >= 60)
            Debug.Log (" Terrain has been generated in " + Mathf.Round (secondsTaken / 60) + " minutes and " + Mathf.Round (secondsTaken) + " seconds.");
        else
            Debug.Log (" Terrain has been generated in " + Mathf.Round (secondsTaken) + " seconds.");
    }
}
