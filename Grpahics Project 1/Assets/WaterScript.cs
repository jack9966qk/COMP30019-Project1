using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterScript : MonoBehaviour {

	public Color color = new Color (137, 218, 251);

	// Use this for initialization
	void Start () {
	}

	public void GenerateMesh(Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight) {
		MeshFilter filter = this.gameObject.AddComponent<MeshFilter> ();

		Mesh m = new Mesh ();
		m.name = "Water";
		List<Vector3> vertices = new List<Vector3>();
		List<Color> colors = new List<Color> ();
		List<Vector3> normals = new List<Vector3> ();

		Vector3 normal1 = Vector3.Cross (topRight - bottomLeft, topLeft - topRight);
		Vector3 normal2 = Vector3.Cross (bottomRight - bottomLeft, topRight - bottomRight);

		vertices.Add (bottomLeft);
		vertices.Add (topRight);
		vertices.Add (topLeft);
		vertices.Add (bottomLeft);
		vertices.Add (bottomRight);
		vertices.Add (topRight);

		colors.Add (color);
		colors.Add (color);
		colors.Add (color);
		colors.Add (color);
		colors.Add (color);
		colors.Add (color);

		normals.Add (normal1);
		normals.Add (normal1);
		normals.Add (normal1);
		normals.Add (normal2);
		normals.Add (normal2);
		normals.Add (normal2);

		m.vertices = vertices.ToArray ();
		m.colors = colors.ToArray ();
		m.normals = normals.ToArray();
		int[] triangles = new int[m.vertices.Length];
		for (int i = 0; i < m.vertices.Length; i++)
			triangles[i] = i;

		m.triangles = triangles;

		filter.mesh = m;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
