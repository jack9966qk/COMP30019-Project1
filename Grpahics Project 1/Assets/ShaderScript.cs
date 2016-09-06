using UnityEngine;
using System.Collections;

public class ShaderScript : MonoBehaviour
{
    public Shader shader;
	public PointLight[] pointLights;

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
		Color[] pointLightColors = new Color[pointLights.Length];
		Vector4[] pointLightPositions = new Vector4[pointLights.Length];
		for (int i = 0; i < pointLights.Length; i++) {
			PointLight light = pointLights [i];
			pointLightColors [i] = light.color;
			pointLightPositions [i] = light.GetWorldPosition ();
		}

		renderer.material.SetInt ("_NumPointLights", pointLights.Length);
 		renderer.material.SetColorArray ("_PointLightColors", pointLightColors);
    }

}
