using UnityEngine;
using System.Collections;

public class ShaderScript : MonoBehaviour
{
	public Shader shader;
	public PointLight pointLight;

	// adjustable parameters 
	public float alpha = 1f;
	public float ambient = 1f;
	public float diffuse = 0.5f;
	public float specular = 0.05f;

	// Use this for initialization
	void Start()
	{
		// Add a MeshRenderer component. This component actually renders the mesh that
		// is defined by the MeshFilter component.
		MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
		renderer.material.shader = shader;
	}

	// Called each frame
	void Update()
	{
		// Get renderer component (in order to pass params to shader)
		MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

		// Pass updated light positions to shader
		renderer.material.SetColor("_PointLightColor", this.pointLight.color);
		renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
		renderer.material.SetFloat ("_Alpha", this.alpha);
		renderer.material.SetFloat ("_Ambient", this.ambient);
		renderer.material.SetFloat ("_Diffuse", this.diffuse);
		renderer.material.SetFloat ("_Specular", this.specular);
	}

}
