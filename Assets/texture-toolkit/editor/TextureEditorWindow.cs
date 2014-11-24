using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

public class TextureEditorWindow : EditorWindow
{
	Texture2D preview;
	int currentToolbar = 0;
	int currentTool = 0;
	bool drawToolOn = false;
	List<Texture2D> pastVersions = new List<Texture2D>();
	VersionManager<Texture2D> versions = new VersionManager<Texture2D> ();
	
	[MenuItem ("Window/Texture TK/Editor")]
	public static void ShowWindow ()
	{
		EditorWindow win = EditorWindow.GetWindow<TextureEditorWindow> ("Tex-Editor");
		win.minSize = new Vector2 (275f, 200f);
	}

	void OnGUI ()
	{
		GUILayout.BeginHorizontal (EditorStyles.toolbar);
			if (GUILayout.Button ("Open", EditorStyles.toolbarButton)){
				preview = LoadTexture (EditorUtility.OpenFilePanel("Open Image", Application.absoluteURL, ""));
				versions.Clear ();
				versions.Add (Instantiate(preview) as Texture2D);
			}
			if (GUILayout.Button ("Save", EditorStyles.toolbarButton)){
				SaveTexture(preview, EditorUtility.SaveFilePanelInProject("Save Texture", "image", "png", ""));
				AssetDatabase.Refresh();
			}
			currentTool = EditorGUILayout.Popup (currentTool, new string[3]{"No Tool", "Brush", "Line"}, EditorStyles.toolbarDropDown);
			if (GUILayout.Button ("Rotate", EditorStyles.toolbarButton)) {
				preview = texturetk.TextureTools.Rotate(preview);
			}
			if (GUILayout.Button ("Flip Y", EditorStyles.toolbarButton)) {
				preview = texturetk.TextureTools.FlipY(preview);
			}
			if (GUILayout.Button ("Undo", EditorStyles.toolbarButton)) {
				versions.Revert();
				preview = Instantiate(versions.currentVersion) as Texture2D;
				Repaint();
			}
		GUILayout.EndHorizontal ();
		GUILayout.Label (preview);
		Rect texrect = GUILayoutUtility.GetLastRect ();
		if (currentTool==1) {
			TryDraw (GUILayoutUtility.GetLastRect ());
		}
	}
	#region Static Helper Methods
	/// <summary>
	/// Saves a texture.
	/// </summary>
	/// <param name="tex">Texture to save</param>
	/// <param name="path">Path to save the texture to</param>
	static void SaveTexture(Texture2D tex, string path)
	{
		if (!string.IsNullOrEmpty(path)){
			byte[] bytes = tex.EncodeToPNG();
			System.IO.File.WriteAllBytes(path, bytes);
		}
	}
	/// <summary>
	/// Loads a texture.
	/// </summary>
	/// <returns>The texture as a Texture2D</returns>
	/// <param name="path">Path to load the texture from.</param>
	static Texture2D LoadTexture(string path)
	{
		Texture2D tex = new Texture2D(0,0);
		if (!string.IsNullOrEmpty(path)){
			byte[] bytes = System.IO.File.ReadAllBytes(path);
			tex.LoadImage(bytes);
		}
		return tex;
	}
	#endregion
	void TryDraw(Rect texrect){
		if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && texrect.Contains (Event.current.mousePosition)) {
			float pixRatio = Mathf.Max(1f, preview.width / texrect.width); // Ratio to convert mouse coordinates to texture pixel coordinates
			int texCursorX = (int)((Event.current.mousePosition.x - texrect.x - 3) * pixRatio);
			int texCursorY = preview.height - (int)((Event.current.mousePosition.y - texrect.y - 3) * pixRatio);
			for (int x=texCursorX-3;x<texCursorX+3;x++){
				for (int y=texCursorY-3;y<texCursorY+3;y++){
					preview.SetPixel(x, y, Color.red);
				}
			}
			preview.Apply();
			Repaint();
		} else if (Event.current.type==EventType.MouseUp){
			versions.Add(Instantiate(preview) as Texture2D);
		}
	}
}

public class VersionManager<T>
{
	List<T> versions;
	public VersionManager(){
		versions = new List<T>();
	}
	public void Add(T version){
		versions.Add (version);
	}
	public void Clear(){
		versions = new List<T> ();
	}
	public void Revert(){
		if (versions.Count>1){
			versions.RemoveAt(versions.Count-1);
		}
	}
	public T currentVersion{
		get{
			return versions[versions.Count-1];
		}
		set{
			versions[-1] = value;
		}
	}
}