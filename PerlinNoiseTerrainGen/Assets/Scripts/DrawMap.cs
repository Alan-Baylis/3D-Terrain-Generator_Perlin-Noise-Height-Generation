using UnityEngine;
using System.Collections;

public class DrawMap : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

	public void DrawMesh(VertexData meshData, Texture2D texture) {
		meshFilter.sharedMesh = meshData.CreateMesh ();
		meshRenderer.sharedMaterial.mainTexture = texture;
	}

}
