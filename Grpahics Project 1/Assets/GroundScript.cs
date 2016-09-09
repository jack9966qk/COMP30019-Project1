using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundScript : MonoBehaviour {

	public GameObject camera;
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
		generateHeightmap (heightMap, length, length, randomMagnitude);


		MeshFilter filter = this.gameObject.AddComponent<MeshFilter> ();
		filter.mesh = this.CeateMesh (heightMap, length, centre);
		this.gameObject.AddComponent<MeshCollider> ();
		setWaterPosition (filter.mesh);
		setCameraPosition (filter.mesh);
		setWallPositions (filter.mesh);
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


	Color colorOfHeight(float y, int height) {
		float factor = (y - centre.y) / height;
		if (factor > 0.5) {
			return high;
		} else if (factor > 0.3) {
			return med;
		} else {
			return low;
		}
	}

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
		
	void getHeightBound(Mesh m, out float min, out float max) {
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

		min = minHeight;
		max = maxHeight;
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

	void setWaterPosition(Mesh m) {
		float ymin, ymax;
		getHeightBound (m, out ymin, out ymax);
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

	void setCameraPosition(Mesh m) {
		float ymin, ymax;
		getHeightBound (m, out ymin, out ymax);
		float oneThird = ymin + (ymax - ymin) / 3;
		Vector3[] vertices = m.vertices;
		Vector3 chosen = vertices[0];
		float lastHeight = Mathf.Abs(chosen.y - oneThird);
		float lastDist = Mathf.Pow(chosen.x, 2) + Mathf.Pow(chosen.z, 2) ;

		foreach (Vector3 vertex in vertices) {
			float h = Mathf.Abs (vertex.y - oneThird);
			float dist = Mathf.Pow(vertex.x, 2)  + Mathf.Pow(vertex.z, 2) ;
			if (h < lastHeight && dist < lastDist) {
				chosen = vertex;
				lastHeight = h;
				lastDist = dist;
			}
		}
		
		camera.transform.position = new Vector3(chosen.x, chosen.y + 40f, chosen.z);
		//Vector3 lookto = new Vector3 (centre.x, height, centre.z);
//		camera.transform.rotation = Quaternion.LookRotation (lookto-camera.transform.position, Vector3.up);
		camera.transform.rotation = Quaternion.Euler(Vector3.zero);
	}


	void setWallPositions(Mesh m) {
		float ymin, ymax;
		getHeightBound (m, out ymin, out ymax);

		float xmin, xmax, zmin, zmax;
		getXZBound (m, out xmin, out xmax, out zmin, out zmax);

		this.backWall.transform.position = new Vector3 (centre.x, centre.y, zmin);
		this.frontWall.transform.position = new Vector3 (centre.x, centre.y, zmax);
		this.leftWall.transform.position = new Vector3 (xmax, centre.y, centre.z);
		this.rightWall.transform.position = new Vector3 (xmin, centre.y, centre.z);
		this.topWall.transform.position = new Vector3 (centre.x, ymax + 200f, centre.z);

	}

}
