texture-toolkit
===================

Easily create, manipulate, and export textures in the Unity Technologies Game Engine

Copyright EJM Software 2015 – [ejmsoftware.com](http://ejmsoftware.com)

------

### EDITOR

Found in editor folder

TextureCreatorWindow
- Open window using: "Window/Texture TK/Creator"
- Use to generate textures procedurally in the Unity Editor

TextureEditorWindow
- Open window using: "Window/Texture TK/Editor"
- Use to edit/draw on textures in the Unity Editor

------

### API

Found in scripts folder and works in runtime

TextureGen
- create procedual textures using algorithms for clouds, marble, wood, and xor

TextureTools
- Draw shapes on textures and export to png

------

### EXAMPLE USES

Export Mesh uvs to png file

	using UnityEngine;
	using texturetk;

	[RequireComponent(typeof(MeshFilter))]
	public class uvexporter : MonoBehaviour {

		[ContextMenu("Export to file")]
		void ExportUVs () {
			// Load the mesh
			Mesh m = gameObject.GetComponent<MeshFilter> ().sharedMesh;
			int[] tris = m.triangles;
			Vector2[] uvs = m.uv;
			// Create the texture to draw the uv map on
			int w = 1024;
			int h = 1024;
			Texture2D tex = new Texture2D(w, h);
			// Draw the uv map lines on the texture
			for (int i=0;i<tris.Length;i+=3){
				Vector2 pt1 = uvs[tris[i]];
				Vector2 pt2 = uvs[tris[i+1]];
				Vector2 pt3 = uvs[tris[i+2]];
				TextureTools.DrawLine(tex, (int)(pt1.x*w), (int)(pt1.y*h), (int)(pt2.x*w), (int)(pt2.y*h), Color.red);
				TextureTools.DrawLine(tex, (int)(pt2.x*w), (int)(pt2.y*h), (int)(pt3.x*w), (int)(pt3.y*h), Color.red);
				TextureTools.DrawLine(tex, (int)(pt3.x*w), (int)(pt3.y*h), (int)(pt1.x*w), (int)(pt1.y*h), Color.red);
			}
			tex.Apply();
			// Save the texture with exported uvs
			string path = Application.dataPath + "/exporteduvs.png";
			Debug.Log ("Exporting uvs to "+path);
			TextureTools.SaveTexture(tex, path);
		}
	}

------

Licensed under Apache 2.0.
You may obtain a copy of the License at [apache.org](http://www.apache.org/licenses/LICENSE-2.0)