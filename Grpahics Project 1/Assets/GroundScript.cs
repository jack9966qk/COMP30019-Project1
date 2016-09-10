using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundScript : MonoBehaviour {

	public GameObject playerCam;
	public GameObject waterPlane;
	public int length = 65;
	public int height = 200;
	public Vector3 centre = new Vector3(0,0,0);
	public float scale = 4.0f;
	public float bottomLeftInit = 0.4f;
	public float bottomRightInit = 0.6f;
	public float topLeftInit = 0.3f;
	public float topRightInit = 0.5f;

	public float randomMagnitude = 0.2f;

	public GameObject frontWall;
	public GameObject backWall;
	public GameObject leftWall;
	public GameObject rightWall;
	public GameObject topWall;

	public Color high = new Color (20, 20, 20);
	public Color med = new Color (0, 5, 0);
	public Color low = new Color (5, 5, 5);

	// Use this for initialization
	void Start () {
		this.transform.position = centre;
		float[,] heightMap = new float[length, length];
		generateHeightmap (heightMap, length, randomMagnitude);

		MeshFilter filter = this.gameObject.AddComponent<MeshFilter> ();
		filter.mesh = this.CeateMesh (heightMap, length, centre);
		this.gameObject.AddComponent<MeshCollider> ();
		setWaterPosition (filter.mesh);
		setCameraPosition (filter.mesh);
		setWallPositions (filter.mesh);
	}

	// helper function to deal with out-of-bound issue
	float? getValue(float?[,] assigned, int x, int y, int maxWidth, int maxHeight, float alternative) {
		if (x < 0 || x >= maxWidth || y < 0 || y >= maxHeight) {
			return alternative;
		} else {
			return assigned [x, y];
		}
	}

	// performs the diamond and square step on a map (x to x+width-1, y to y+width-1)
	void performDiamondSquare(float?[,] assigned, int x, int y, int length, float ranMagnitude, int maxLength) {

		int l = length - 1;
		float bl, br, tl, tr;
		bl = (float) assigned[x, y];
		br = (float) assigned[x + l, y];
		tl = (float) assigned[x, y + l];
		tr = (float) assigned[x + l, y + l];

		// diamond
		float centre = ((bl + tr + tl + tr) / 4) + (Random.value - 0.5f) * ranMagnitude;
		assigned[x + l / 2, y + l / 2] = centre;


		// square
		float? left = getValue(assigned, x - l / 2, y + l / 2, maxLength, maxLength, centre);
		float? right = getValue(assigned, x + (l / 2) * 3, y + l / 2, maxLength, maxLength, centre);
		float? up = getValue(assigned, x + l / 2, y + (l / 2) * 3, maxLength, maxLength, centre);
		float? down = getValue(assigned, x + l / 2, y - l / 2, maxLength, maxLength, centre);

		assigned[x, y + l / 2] = ((bl + tl + centre + left ?? centre) / 4) + (Random.value - 0.5f) * ranMagnitude;
		assigned[x + l / 2, y] = ((bl + br + centre + down ?? centre) / 4) + (Random.value - 0.5f) * ranMagnitude;
		assigned[x + l / 2, y + l] = ((tl + tr + centre + up ?? centre) / 4) + (Random.value - 0.5f) * ranMagnitude;
		assigned[x + l, y + l / 2] = ((br + tr + centre + right ?? centre) / 4) + (Random.value - 0.5f) * ranMagnitude;

	}



	// generate a heightmap with diamond square on range (x to x+width-1, y to y+width-1)
	void generateHeightmap(float [,] map, int length, float ranMagnitude) {

		float?[,] assigned = new float?[length, length];

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				assigned [i, j] = null;
			}
		}

		assigned[0, 0] = bottomLeftInit;
		assigned[length - 1, 0] = bottomRightInit;
		assigned[0, length - 1] = topLeftInit;
		assigned[length - 1, length - 1] = topRightInit;

		

		int wid = length - 1;
		int hei = length - 1;
		float ranMag = ranMagnitude;

		int loop = 1;

		while (wid >= 2 && hei >= 2) {
			for (int i = 0; i < loop; i++) {
				for (int j = 0; j < loop; j++) {
					performDiamondSquare (assigned, wid * i, hei * j, wid+1, ranMag, length);
				}
			}

			loop *= 2;
			wid = wid / 2;
			hei = hei / 2;
			ranMag = ranMag / 2.0f;
		}

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				map [i, j] = (float) assigned [i, j];
			}
		}

	}

	// vertex color based on height
	Color colorOfHeight(float y, int height) {
		float factor = (y - centre.y) / height;
		if (factor > 0.5f) {
			return high;
		} else if (factor > 0.35f) {
			return med;
		} else {
			return low;
		}
	}

	// create mesh of the ground based on heightmap
	Mesh CeateMesh(float[,] heightmap, int length, Vector3 centre) {
		Mesh m = new Mesh ();
		m.name = "Ground";

		int half = (length + 1) / 2;
		float cx = centre.x;
		float cy = centre.y;
		float cz = centre.z;

		List<Vector3> vertices = new List<Vector3>();
		List<Color> colors = new List<Color> ();
		List<Vector3> normals = new List<Vector3> ();


		for (int i = 0; i < length - 1; i++) {
			for (int j = 0; j < length-1; j++) {
				Vector3 topLeft = new Vector3 (cx + (i - half) * scale, cy + height * heightmap [i, j], cz + (j - half) * scale);
				Vector3 topRight = new Vector3 (cx + (i + 1 - half) * scale, cy + height * heightmap [i + 1, j], cz + (j - half) * scale);
				Vector3 bottomLeft = new Vector3 (cx + (i - half) * scale, cy + height * heightmap [i, j + 1], cz + (j + 1 - half) * scale);
				Vector3 bottomRight = new Vector3 (cx + (i+1 - half) * scale, cy + height * heightmap [i+1, j + 1], cz + (j + 1 - half) * scale);

				Vector3 normal1 = Vector3.Cross (topRight - bottomLeft, topLeft - topRight);
				Vector3 normal2 = Vector3.Cross (bottomRight - bottomLeft, topRight - bottomRight);

				vertices.Add (bottomLeft);
				vertices.Add (topRight);
				vertices.Add (topLeft);
				vertices.Add (bottomLeft);
				vertices.Add (bottomRight);
				vertices.Add (topRight);

				colors.Add (colorOfHeight(bottomLeft.y, height));
				colors.Add (colorOfHeight(topRight.y, height));
				colors.Add (colorOfHeight(topLeft.y, height));
				colors.Add (colorOfHeight(bottomLeft.y, height));
				colors.Add (colorOfHeight(bottomRight.y, height));
				colors.Add (colorOfHeight(topRight.y, height));

				normals.Add (normal1);
				normals.Add (normal1);
				normals.Add (normal1);
				normals.Add (normal2);
				normals.Add (normal2);
				normals.Add (normal2);
			}
		}


		m.vertices = vertices.ToArray ();
		m.colors = colors.ToArray ();
		m.normals = normals.ToArray();
		int[] triangles = new int[m.vertices.Length];
		for (int i = 0; i < m.vertices.Length; i++)
			triangles[i] = i;

		m.triangles = triangles;

		return m;
	}


	// helper function to get bounds of a mesh

	void getYBound(Mesh m, out float ymin, out float ymax) {
		Vector3[] vertices = m.vertices;
		float minHeight = vertices [0].y;
		float maxHeight = vertices [0].y;
		foreach (Vector3 vertex in vertices) {
			if (vertex.y < minHeight) {
				minHeight = vertex.y;
			} else if (vertex.y > maxHeight) {
				maxHeight = vertex.y;
			}
		}

		ymin = minHeight;
		ymax = maxHeight;
	}
		
	void getXZBound(Mesh m, out float xmin, out float xmax, out float zmin, out float zmax) {
		Vector3[] vertices = m.vertices;
		float minX = vertices [0].x;
		float maxX = vertices [0].x;
		float minZ = vertices [0].z;
		float maxZ = vertices [0].z;
		foreach (Vector3 vertex in vertices) {
			if (vertex.x < minX) {
				minX = vertex.x;
			} else if (vertex.x > maxX) {
				maxX = vertex.x;
			}

			if (vertex.z < minZ) {
				minZ = vertex.z;
			} else if (vertex.z > maxZ) {
				maxZ = vertex.z;
			}

		}

		xmin = minX;
		xmax = maxX;
		zmin = minZ;
		zmax = maxZ;
	}

	// set position of water plane
	void setWaterPosition(Mesh m) {
		float ymin, ymax;
		getYBound (m, out ymin, out ymax);
		float height = ymin + (ymax - ymin) / 3;

		float xmin, xmax, zmin, zmax;
		getXZBound (m, out xmin, out xmax, out zmin, out zmax);
		waterPlane.GetComponent<WaterScript> ().GenerateMesh (
			new Vector3 (xmin, 0, zmin),
			new Vector3 (xmax, 0, zmin),
			new Vector3 (xmin, 0, zmax),
			new Vector3 (xmax, 0, zmax));
		waterPlane.gameObject.transform.position = new Vector3 (centre.x, height, centre.z);
	}

	// set position of camera (player)
	void setCameraPosition(Mesh m) {
		float ymin, ymax;
		getYBound (m, out ymin, out ymax);
		float oneThird = ymin + (ymax - ymin) / 3;
		Vector3[] vertices = m.vertices;
		Vector3 chosen = vertices[0];
		float lastHeight = Mathf.Abs(chosen.y - oneThird);
		float lastDist = Mathf.Pow(chosen.x, 2) + Mathf.Pow(chosen.z, 2) ;

		// put camera in a low to medium height, as close to centre as possible
		foreach (Vector3 vertex in vertices) {
			float h = Mathf.Abs (vertex.y - oneThird);
			float dist = Mathf.Pow(vertex.x, 2)  + Mathf.Pow(vertex.z, 2) ;
			if (h < lastHeight && dist < lastDist) {
				chosen = vertex;
				lastHeight = h;
				lastDist = dist;
			}
		}
		
		playerCam.transform.position = new Vector3(chosen.x, chosen.y + 40f, chosen.z);
		playerCam.transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	// set position of walls
	void setWallPositions(Mesh m) {
		float ymin, ymax;
		getYBound (m, out ymin, out ymax);

		float xmin, xmax, zmin, zmax;
		getXZBound (m, out xmin, out xmax, out zmin, out zmax);

		this.backWall.transform.position = new Vector3 (centre.x, centre.y, zmin);
		this.frontWall.transform.position = new Vector3 (centre.x, centre.y, zmax);
		this.leftWall.transform.position = new Vector3 (xmax, centre.y, centre.z);
		this.rightWall.transform.position = new Vector3 (xmin, centre.y, centre.z);
		this.topWall.transform.position = new Vector3 (centre.x, ymax + 200f, centre.z);
	}

}
