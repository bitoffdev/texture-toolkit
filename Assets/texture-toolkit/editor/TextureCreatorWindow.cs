using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using texturetk;
using System.Collections.Generic;

public class TextureCreatorWindow : EditorWindow
{
	class TextureLayer
	{
		public MethodInfo LayerMethod;
		public Color StartColor = Color.white;
		public Color EndColor = Color.black;

		object[] ParameterData = new object[0];
		Texture2D _tex = new Texture2D(512, 512);
		bool isDirty = true;

		public TextureLayer(MethodInfo layermethod){
			LayerMethod = layermethod;
			ParameterData = loadParameters(LayerMethod);
		}

		public void SetDirty(){
			isDirty = true;
		}

		public string Name{
			get {
				return LayerMethod.Name;
			}
		}

		public Texture2D Tex{
			get {
				if (isDirty){
					_tex = LayerMethod.Invoke(null, ParameterData) as Texture2D;
					TextureTools.GrayscaleToColor(_tex, StartColor, EndColor);
					isDirty = false;
				}
				return _tex;
			}
		}

		public void DrawEditor(){
			EditorGUI.BeginChangeCheck ();
			GUI.Label (new Rect (3, 20, 70, 16), "Start Color");
			StartColor = EditorGUI.ColorField (new Rect(76, 20, 80, 16), StartColor);
			GUI.Label (new Rect (3, 40, 70, 16), "End Color");
			EndColor = EditorGUI.ColorField (new Rect(76, 40, 80, 16), EndColor);

			ParameterInfo[] Params = LayerMethod.GetParameters ();
			for (int i=0;i<Params.Length;i++){
				if (Params[i].Name=="w"){
					ParameterData[i] = _tex.width;
				} else if (Params[i].Name=="h"){
					ParameterData[i] = _tex.height;
				} else {
					GUI.Label (new Rect(3, i*20+20, 70, 16), Params[i].Name);
					if (Params[i].ParameterType==typeof(int)){
						ParameterData[i] = EditorGUI.IntField(new Rect(76, i*20+20, 80, 16), (int)ParameterData[i]);
					}
					if (Params[i].ParameterType==typeof(float)){
						ParameterData[i] = EditorGUI.FloatField(new Rect(76, i*20+20, 80, 16), (float)ParameterData[i]);
					}
				}
			}
			if (EditorGUI.EndChangeCheck()){
				SetDirty();
			}
		}
	}

	MethodInfo[] LayerMethods;
	string[] LayerMethodNames;
	int NewMethodIndex = 0;
	List<TextureLayer> layers = new List<TextureLayer>();
	
	//Structure
	Vector2 ScrollPos = Vector2.zero;
	Rect PreviewRect;
	Rect LabelRect = new Rect(3, 3, 50, 16);
	Rect DeleteRect = new Rect(76, 3, 80, 16);

	[MenuItem ("Window/Texture TK/Creator")]
	public static void ShowWindow ()
	{
		EditorWindow win = EditorWindow.GetWindow<TextureCreatorWindow> ("Tex-Creator");
		win.minSize = new Vector2 (160f, 200f);
	}

	void OnEnable()
	{
		// Get the public methods.
		LayerMethods = loadMethods (typeof(TextureGen));
		//Get method names
		LayerMethodNames = new string[LayerMethods.Length];
		for (int i=0; i<LayerMethodNames.Length; i++) {
			LayerMethodNames[i] = LayerMethods[i].Name;
		}
	}

	void OnGUI ()
	{
		//Toolbar
		GUILayout.BeginHorizontal (EditorStyles.toolbar);
		NewMethodIndex = EditorGUILayout.Popup (NewMethodIndex, LayerMethodNames, EditorStyles.toolbarPopup);
		if (GUILayout.Button ("New Layer", EditorStyles.toolbarButton)) {
			layers.Add(new TextureLayer(LayerMethods[NewMethodIndex]));
		}
		if (GUILayout.Button ("Save", EditorStyles.toolbarButton, GUILayout.Width (50f))){
			SaveTexture(CombinedLayers(layers), EditorUtility.SaveFilePanelInProject("Save Texture", "Texture", "png", ""));
			AssetDatabase.Refresh();
		}
		GUILayout.EndHorizontal ();
		//Body
		int size = Mathf.Min (200, (int)position.width);
		PreviewRect = new Rect (0, 0, position.width, size);

		ScrollPos = GUI.BeginScrollView (new Rect(0, 16, position.width, position.height-16), ScrollPos, new Rect(0, 0, position.width-15, layers.Count*150+size));
		for (int i=0; i<layers.Count; i++) {
			GUI.BeginGroup(new Rect(0, i*150+size, position.width, 145), EditorStyles.textArea);
			GUI.Label(LabelRect, layers[i].Name);
			if (GUI.Button(DeleteRect, "Delete")){
				layers.RemoveAt(i);
				return;
			}
			layers[i].DrawEditor();
			GUI.EndGroup();

			GUI.DrawTexture(PreviewRect, layers[i].Tex, ScaleMode.ScaleToFit);
		}
		GUI.EndScrollView ();
	}

	#region Static Helper Methods
	/// <summary>
	/// Loads the methods for a given type.
	/// </summary>
	/// <returns>The methods as a MethodInfo Array.</returns>
	/// <param name="type">Type.</param>
	static MethodInfo[] loadMethods(Type type){
		MethodInfo[] methods = type.GetMethods();
		methods = System.Array.FindAll (methods, p => p.DeclaringType==type);
		return methods;
	}
	/// <summary>
	/// Loads the parameters of a given method
	/// </summary>
	/// <returns>ParameterInfo Array</returns>
	/// <param name="method">Method to get paramters from</param>
	static object[] loadParameters(MethodInfo method){
		ParameterInfo[] pInfo = method.GetParameters();
		object[] pData = new object[pInfo.Length];
		for (int i=0; i<pData.Length; i++) {
			if (pInfo[i].IsOptional){
				pData[i] = pInfo[i].DefaultValue;
			} else if (pInfo[i].ParameterType.IsValueType){
				pData[i] = Activator.CreateInstance(pInfo[i].ParameterType);
			} else {
				pData[i] = null;
			}
		}
		return pData;
	}
	static Texture2D CombinedLayers(List<TextureLayer> ls){
		Texture2D tex = new Texture2D (512, 512);
		Color[] outpix = new Color[tex.width*tex.height];
		for (int i=0;i<outpix.Length;i++){
			outpix[i] = Color.clear;
		}
		foreach (TextureLayer layer in ls) {
			Color[] pix = layer.Tex.GetPixels();
			for (int i=0;i<pix.Length;i++){
				outpix[i] = Color.Lerp(outpix[i], pix[i], pix[i].a);
			}
		}
		tex.SetPixels (outpix);
		return tex;
	}
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
}
