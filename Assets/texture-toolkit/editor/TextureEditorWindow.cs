using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

public class TextureEditorWindow : EditorWindow
{
	Texture2D preview;
	Rect labelposition;
	int currentTool = 0;
	List<Texture2D> versions = new List<Texture2D> ();
	Vector2 clickpos;
	Rect settingsrect = new Rect (10f, 30f, 100f, 40f);
	//Styles
	//GUIStyle winstyle;
	//User Prefs
	Color paintColor = Color.white;
	
	[MenuItem ("Window/Texture TK/Editor")]
	public static void ShowWindow ()
	{
		EditorWindow win = EditorWindow.GetWindow<TextureEditorWindow> ("Tex-Editor");
		win.minSize = new Vector2 (275f, 200f);
	}

	void OnGUI ()
	{
		GUILayout.BeginHorizontal (EditorStyles.toolbar);
			if (GUILayout.Button ("File", EditorStyles.toolbarDropDown, GUILayout.Width (50f))) {
				GenericMenu filedrop = new GenericMenu();
				filedrop.AddItem(new GUIContent("New"), false, NewFile);
				filedrop.AddItem(new GUIContent("Open"), false, OpenFile);
				filedrop.AddItem(new GUIContent("Save"), false, SaveFile);
				filedrop.DropDown(new Rect(5f, 0f, 80f, 20f));
			}
		if (GUILayout.Button ("Edit", EditorStyles.toolbarDropDown, GUILayout.Width (50f))) {
			GenericMenu editdrop = new GenericMenu();
			editdrop.AddItem(new GUIContent("Rotate"), false, RotateTex);
			editdrop.AddItem(new GUIContent("Flip Y"), false, FlipTex);
			editdrop.AddItem(new GUIContent("Undo"), false, UndoTex);
			editdrop.DropDown(new Rect(55f, 0f, 80f, 20f));
		}
		currentTool = EditorGUILayout.Popup (currentTool, new string[5]{"Tools", "Brush", "Line", "Rect", "Circle"}, EditorStyles.toolbarDropDown, GUILayout.Width (50f));
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		
		GUILayout.Label (preview);
		labelposition = GUILayoutUtility.GetLastRect();
		/*
		GUIStyle style = new GUIStyle();
		labelposition = GUILayoutUtility.GetRect(this.position.width, this.position.width);
		GUI.DrawTexture(labelposition, preview, ScaleMode.StretchToFill, true, 10.0f);
		*/
		
		if (currentTool==1) {
			TryDraw (GUILayoutUtility.GetLastRect ());
		} else if (currentTool==2) {
			TryLine (GUILayoutUtility.GetLastRect ());
		} else if (currentTool==3) {
			TryRect (GUILayoutUtility.GetLastRect ());
		} else if (currentTool==4) {
			TryCircle (GUILayoutUtility.GetLastRect ());
		}
		BeginWindows ();
		settingsrect = GUI.Window(0, settingsrect, RectSettings, "Settings");
		EndWindows ();
		if (preview!=null){
			GUI.Box (new Rect (this.position.width-100f, this.position.height-20f, 100f, 20f), TexMousePos().ToString ());
			Repaint ();
		}
	}
	void OpenFile(){
		preview = LoadTexture (EditorUtility.OpenFilePanel("Open Image", Application.absoluteURL, ""));
		versions.Clear ();
		versions.Add(Instantiate(preview) as Texture2D);
	}
	void NewFile(){
		preview = new Texture2D(512, 512);
		versions.Clear ();
		versions.Add(Instantiate(preview) as Texture2D);
	}
	void SaveFile(){
		SaveTexture(preview, EditorUtility.SaveFilePanelInProject("Save Texture", "image", "png", ""));
		AssetDatabase.Refresh();
	}
	void RotateTex(){
		texturetk.TextureTools.Rotate(preview);
		versions.Add(Instantiate(preview) as Texture2D);
	}
	void FlipTex(){
		texturetk.TextureTools.FlipY(preview);
		versions.Add(Instantiate(preview) as Texture2D);
	}
	void UndoTex(){
		if(versions.Count>1){
			versions.RemoveAt(versions.Count-1);
			preview = Instantiate(versions[versions.Count-1]) as Texture2D;
			Repaint();
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
	Vector2 TexMousePos(){
		float pixRatio = Mathf.Max(1f, preview.width / labelposition.width); // Ratio to convert mouse coordinates to texture pixel coordinates
		int texCursorX = (int)((Event.current.mousePosition.x - labelposition.x - 3) * pixRatio);
		int texCursorY = preview.height - (int)((Event.current.mousePosition.y - labelposition.y - 3) * pixRatio);
		return new Vector2((float)texCursorX, (float)texCursorY);
	}
	
	void TryDraw(Rect texrect){
		if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && texrect.Contains (Event.current.mousePosition)) {
			float pixRatio = Mathf.Max(1f, preview.width / texrect.width); // Ratio to convert mouse coordinates to texture pixel coordinates
			int texCursorX = (int)((Event.current.mousePosition.x - texrect.x - 3) * pixRatio);
			int texCursorY = preview.height - (int)((Event.current.mousePosition.y - texrect.y - 3) * pixRatio);
			for (int x=texCursorX-3;x<texCursorX+3;x++){
				for (int y=texCursorY-3;y<texCursorY+3;y++){
					preview.SetPixel(x, y, paintColor);
				}
			}
			preview.Apply();
			Repaint();
		} else if (Event.current.type==EventType.MouseUp){
			versions.Add(Instantiate(preview) as Texture2D);
		}
	}
	void TryLine(Rect texrect){
		if (Event.current.isMouse && texrect.Contains (Event.current.mousePosition)){
			float pixRatio = Mathf.Max(1f, preview.width / texrect.width); // Ratio to convert mouse coordinates to texture pixel coordinates
			int texCursorX = (int)((Event.current.mousePosition.x - texrect.x - 3) * pixRatio);
			int texCursorY = preview.height - (int)((Event.current.mousePosition.y - texrect.y - 3) * pixRatio);
			if (Event.current.type == EventType.MouseDown) {
				clickpos = new Vector2(texCursorX, texCursorY);
			} else if (Event.current.type==EventType.MouseUp){
				texturetk.TextureTools.DrawLine(preview, clickpos, new Vector2(texCursorX, texCursorY), paintColor);
				versions.Add(Instantiate(preview) as Texture2D);
				Repaint ();
			}
		}
	}
	void TryRect(Rect texrect){
		if (Event.current.isMouse && texrect.Contains (Event.current.mousePosition)){
			float pixRatio = Mathf.Max(1f, preview.width / texrect.width); // Ratio to convert mouse coordinates to texture pixel coordinates
			int texCursorX = (int)((Event.current.mousePosition.x - texrect.x - 3) * pixRatio);
			int texCursorY = preview.height - (int)((Event.current.mousePosition.y - texrect.y - 3) * pixRatio);
			if (Event.current.type == EventType.MouseDown) {
				clickpos = new Vector2(texCursorX, texCursorY);
			} else if (Event.current.type==EventType.MouseUp){
				texturetk.TextureTools.DrawRect(preview, clickpos, new Vector2(texCursorX, texCursorY), paintColor);
				versions.Add(Instantiate(preview) as Texture2D);
				Repaint ();
			}
		}
	}
	void TryCircle(Rect texrect){
		if (Event.current.isMouse && texrect.Contains (Event.current.mousePosition)){
			if (Event.current.type == EventType.MouseDown) {
				clickpos = TexMousePos();
			} else if (Event.current.type==EventType.MouseUp){
				texturetk.TextureTools.DrawCircle(preview, clickpos, (int)Vector2.Distance(clickpos, TexMousePos()), paintColor);
				versions.Add(Instantiate(preview) as Texture2D);
				Repaint ();
			}
		}
	}
	void RectSettings(int windowID){
		GUI.DragWindow (new Rect(0,0,1000,20));
		paintColor = EditorGUILayout.ColorField (paintColor);
	}
}