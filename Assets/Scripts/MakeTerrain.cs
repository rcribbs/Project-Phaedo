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
	
    public int resolution = 1024;
    public float zoom = 1f; 
    public float offset = 0f; 
	public double frequency = 1;
	public double lacunarity = 2;
	public double persistence = .5;
	public int octaves = 6;
	public int seed = 0;
	public string qualitySetting = "medium";
	public Terrain terrain;

	// Use this for initialization
	void Start ()
	{
		GenerateTerrain();
	}
	
	void GenerateTerrain()
	{
		QualityMode quality;
		//Translate human readable text to QualityMode data types.
		switch(qualitySetting.ToLower())
		{
			case "low":
				quality = QualityMode.Low;
				break;
			case "high":
				quality = QualityMode.High;
				break;
			default:
				quality = QualityMode.Medium;
			break;
		}
		// Create and set up a perlin generator with the given parameters.
		Perlin theGenerator = new Perlin(frequency, lacunarity, persistence, octaves, seed, quality);
		noiseMap = new Noise2D(resolution, resolution, theGenerator);
		noiseMap.GeneratePlanar(
            offset + -1 * 1/zoom, 
            offset + offset + 1 * 1/zoom, 
            offset + -1 * 1/zoom,
            offset + 1 * 1/zoom);
		// Get the texture version of the Noise2D returned by LibNoise.
		Texture2D textureResult = noiseMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
		textureResult.Apply();
		//File.WriteAllBytes(Application.dataPath + "/../test.png", textureResult.EncodeToPNG() );
		// Create a map to hold the height data that will mapped to the terrainData.
		float[,] heightMap = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapWidth);
		// Map the grayscale value of the Color objects in the texture returned from LibNoise
		// and map the value to the heightMap.
		// NOTE: Need a more effecient way of doing this!
		for(int index = 0; index < terrain.terrainData.heightmapWidth; index++)
		{
			for(int index2 = 0; index2 < terrain.terrainData.heightmapWidth; index2++)
				heightMap[index, index2] = textureResult.GetPixel(index,index2).grayscale;
		}
		// Map the heightmap we created to the terrain data.
		terrain.terrainData.SetHeights(0, 0, heightMap);
		// Bubble up the changes to the terrain object.
		terrain.Flush ();
		// Jump for joy.
		Debug.Log("Terrain has been generated.");
	}
}
