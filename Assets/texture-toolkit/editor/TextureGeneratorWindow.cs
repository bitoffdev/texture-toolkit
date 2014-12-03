using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using texturetk;

public class TextureGeneratorWindow : EditorWindow
{
	bool showSettings = true;
	bool showPreview = true;
	Texture2D preview;
	MethodInfo[] myArrayMethodInfo;
	string[] methodNames;
	int selectedMethod = 0;
	object[] parameterData = new object[0];
	Color startColor = Color.black;
	Color endColor = Color.white;
	
	[MenuItem ("Window/Texture TK/Generator")]
	public static void ShowWindow ()
	{
		EditorWindow.GetWindow<TextureGeneratorWindow> ("Tex-Generator");
	}

	void OnEnable()
	{
		// Get the public methods.
		myArrayMethodInfo = loadMethods (typeof(TextureGen));
		//Get method names
		methodNames = new string[myArrayMethodInfo.Length];
		for (int i=0; i<methodNames.Length; i++) {
			methodNames[i] = myArrayMethodInfo[i].Name;
		}
		parameterData = loadParameters(myArrayMethodInfo[selectedMethod]);
	}
	
	void OnGUI ()
	{
		GUILayout.BeginHorizontal (EditorStyles.toolbar);
		if (GUILayout.Button ("Generate", EditorStyles.toolbarButton)){
			preview = myArrayMethodInfo[selectedMethod].Invoke(null, parameterData) as Texture2D;
			TextureTools.GrayscaleToColor(preview, startColor, endColor);
		}
		if (GUILayout.Button ("Save", EditorStyles.toolbarButton)){
			SaveTexture(preview, EditorUtility.SaveFilePanelInProject("Save Texture", methodNames[selectedMethod], "png", ""));
			AssetDatabase.Refresh();
		}
		GUILayout.EndHorizontal ();
		showSettings = GUILayout.Toggle (showSettings, "Settings", EditorStyles.foldout);
		if (showSettings){
			GUILayout.Box ("Deformation", GUILayout.ExpandWidth(true));
			EditorGUI.BeginChangeCheck ();
			selectedMethod = EditorGUILayout.Popup(selectedMethod, methodNames);
			if (EditorGUI.EndChangeCheck ()) {
				parameterData = loadParameters(myArrayMethodInfo[selectedMethod]);
			}
			ParameterInfo[] parameters = myArrayMethodInfo[selectedMethod].GetParameters();
			for (int i=0;i<parameters.Length;i++){
				GUILayout.BeginHorizontal();
				GUILayout.Label (parameters[i].Name, GUILayout.Width (80f));
				if (parameters[i].ParameterType==typeof(int)){
					parameterData[i] = EditorGUILayout.IntField((int)parameterData[i]);
				}
				if (parameters[i].ParameterType==typeof(float)){
					parameterData[i] = EditorGUILayout.FloatField((float)parameterData[i]);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.Box ("Color Scheme", GUILayout.ExpandWidth(true));
			startColor = EditorGUILayout.ColorField (startColor);
			endColor = EditorGUILayout.ColorField (endColor);
		}
		showPreview = GUILayout.Toggle (showPreview, "Preview", EditorStyles.foldout);
		if (showPreview) {
			GUILayout.Label (preview);
		}
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