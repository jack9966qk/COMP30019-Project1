using UnityEngine;
using System.Collections;

public class GenerateHeightmap : MonoBehaviour {

	public float bottomLeftInit = 0.4f;
	public float bottomRightInit = 0.6f;
	public float topLeftInit = 0.3f;
	public float topRightInit = 0.5f;

	public float randomMagnitude = 0.2f;

	private TerrainData terrainData;

	// Use this for initialization
	void Start () {
		terrainData = this.GetComponent<Terrain>().terrainData;
		int width = terrainData.heightmapWidth;
		int height = terrainData.heightmapHeight;

		float[,] heightMap = new float[width, height];




		generateHeightmap (heightMap, width, height, randomMagnitude);

		terrainData.SetHeights (0, 0, heightMap);
	}

	float? getValue(float?[,] assigned, int x, int y, int maxWidth, int maxHeight, float alternative) {
		if (x < 0 || x >= maxWidth || y < 0 || y >= maxHeight) {
			return alternative;
		} else {
			return assigned [x, y];
		}
	}

	// performs the diamond and square step on a map (x to x+width-1, y to y+width-1)
	void performDiamondSquare(float?[,] assigned, int x, int y, int width, int height, float ranMagnitude, int maxWidth, int maxHeight) {

		int w = width - 1;
		int h = height - 1;
		float bl, br, tl, tr;
		bl = (float) assigned[x, y];
		br = (float) assigned[x + w, y];
		tl = (float) assigned[x, y + h];
		tr = (float) assigned[x + w, y + h];

		// diamond
		float centre = ((bl + tr + tl + tr) / 4) + (Random.value - 0.5f) * ranMagnitude;
		assigned[x + w / 2, y + h / 2] = centre;


		// square
		float? left = getValue(assigned, x - w / 2, y + h / 2, maxWidth, maxHeight, centre);
		float? right = getValue(assigned, x + (w / 2) * 3, y + h / 2, maxWidth, maxHeight, centre);
		float? up = getValue(assigned, x + w / 2, y + (h / 2) * 3, maxWidth, maxHeight, centre);
		float? down = getValue(assigned, x + w / 2, y - h / 2, maxWidth, maxHeight, centre);

		assigned[x, y + h / 2] = ((bl + tl + centre + left ?? centre) / 4) + (Random.value - 0.5f) * ranMagnitude;
		assigned[x + w / 2, y] = ((bl + br + centre + down ?? centre) / 4) + (Random.value - 0.5f) * ranMagnitude;
		assigned[x + w / 2, y + h] = ((tl + tr + centre + up ?? centre) / 4) + (Random.value - 0.5f) * ranMagnitude;
		assigned[x + w, y + h / 2] = ((br + tr + centre + right ?? centre) / 4) + (Random.value - 0.5f) * ranMagnitude;

	}




	// generate a heightmap with diamond square on range (x to x+width-1, y to y+width-1)
	void generateHeightmap(float [,] map, int width, int height, float ranMagnitude) {

		float?[,] assigned = new float?[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				assigned [i, j] = null;
			}
		}

		assigned[0, 0] = bottomLeftInit;
		assigned[width - 1, 0] = bottomRightInit;
		assigned[0, height - 1] = topLeftInit;
		assigned[width - 1, height - 1] = topRightInit;

		

		int wid = width - 1;
		int hei = height - 1;
		float ranMag = ranMagnitude;

		int loop = 1;

		while (wid >= 2 && hei >= 2) {
			for (int i = 0; i < loop; i++) {
				for (int j = 0; j < loop; j++) {
					performDiamondSquare (assigned, wid * i, hei * j, wid+1, hei+1, ranMag, width, height);
				}
			}

			loop *= 2;
			wid = wid / 2;
			hei = hei / 2;
			ranMag = ranMag / 2.0f;
		}

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				map [i, j] = (float) assigned [i, j];
			}
		}

	}



}
