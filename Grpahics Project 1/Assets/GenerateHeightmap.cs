using UnityEngine;
using System.Collections;

public class GenerateHeightmap : MonoBehaviour {

	public float bottomLeftInit = 0.4f;
	public float bottomRightInit = 0.6f;
	public float topLeftInit = 0.3f;
	public float topRightInit = 0.5f;

	public float randomMagnitude = 0.2f;

	private TerrainData terrainData;

	private int a = 0;

	// Use this for initialization
	void Start () {
		terrainData = this.GetComponent<Terrain> ().terrainData;
		int width = terrainData.heightmapWidth;
		int height = terrainData.heightmapHeight;

		float[,] heightMap = new float[width, height];

//		for (int i = 0; i < width; i++) {
//			for (int j = 0; j < height; j++) {
//				heightMap [i, j] = -10000f;
//			}
//		}
			
		heightMap [0, 0] = bottomLeftInit;
		heightMap [width - 1, 0] = bottomRightInit;
		heightMap [0, height - 1] = topLeftInit;
		heightMap [width - 1, height - 1] = topRightInit;

		generateHeightmap (heightMap, width, height, randomMagnitude);
	
		terrainData.SetHeights (0, 0, heightMap);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// performs the diamond and square step on a map (x to x+width-1, y to y+width-1)
	void performDiamondSquare(float [,] map, int x, int y, int width, int height, float ranMagnitude) {

		int w = width - 1;
		int h = height - 1;
		float bl, br, tl, tr;
		bl = map [x, y];
		br = map [x + w, y];
		tl = map [x, y + h];
		tr = map [x + w, y + h];

		// diamond
		float centre = ((bl + tr + tl + tr) / 4) + Random.value * ranMagnitude;
		map [x + w/2, y + h/2] = centre;

		// square
		map[x, y + h/2] = ((bl + tl + centre + centre) /4) + Random.value * ranMagnitude;
		map[x + w/2, y] = ((bl + br + centre + centre) / 4) + Random.value * ranMagnitude;
		map[x + w/2, y + h] = ((tl + tr + centre + centre) / 4) + Random.value * ranMagnitude;
		map[x + w, y + h/2] = ((br + tr + centre + centre) / 4) + Random.value * ranMagnitude;

	}


	// generate a heightmap with diamond square on range (x to x+width-1, y to y+width-1)
	void generateHeightmap(float [,] map, int width, int height, float ranMagnitude) {

		int wid = width - 1;
		int hei = height - 1;
		float ranMag = ranMagnitude;

		int loop = 1;

		while (wid >= 2 && hei >= 2) {
			for (int i = 0; i < loop; i++) {
				for (int j = 0; j < loop; j++) {
					performDiamondSquare (map, wid * i, hei * j, wid+1, hei+1, ranMag);
				}
			}

			loop *= 2;
			wid = wid / 2;
			hei = hei / 2;
			ranMag = ranMag / 2.0f;
		}

	}
	
}
