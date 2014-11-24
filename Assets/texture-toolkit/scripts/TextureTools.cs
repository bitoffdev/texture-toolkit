using UnityEngine;
using System.Collections;

namespace texturetk
{
	public class TextureTools
	{
		public static Texture2D FlipY(Texture2D tex){
			Color[] pix = new Color[tex.width*tex.height];
			for (int x=0;x<tex.width;x++){
				for (int y=0;y<tex.height;y++){
					pix[x + y * tex.width] = tex.GetPixel(x, tex.height - y);
				}
			}
			Texture2D newTex = new Texture2D (tex.width, tex.height);
			newTex.SetPixels(pix);
			newTex.Apply();
			return newTex;
		}
		public static Texture2D Rotate(Texture2D tex){
			Color[] pix = new Color[tex.width*tex.height];
			for (int x=0;x<tex.width;x++){
				for (int y=0;y<tex.height;y++){
					pix[x + y * tex.width] = tex.GetPixel(tex.height - y, x);
				}
			}
			Texture2D newTex = new Texture2D (tex.width, tex.height);
			newTex.SetPixels(pix);
			newTex.Apply();
			return newTex;
		}
		/// <summary>
		/// Converts Grayscale Textures to Colored Texture
		/// </summary>
		/// <returns>Converted Texture2D</returns>
		/// <param name="tex">Texture2D to Convert</param>
		/// <param name="start">Start Color</param>
		/// <param name="end">End Color</param>
		public static Texture2D GrayscaleToColor(Texture2D tex, Color start, Color end)
		{
			Color[] pix = tex.GetPixels ();
			for (int i=0; i<pix.Length; i++) {
				pix[i] = Color.Lerp (start, end, pix[i].grayscale);
			}
			Texture2D newTex = new Texture2D (tex.width, tex.height);
			newTex.SetPixels(pix);
			newTex.Apply();
			return newTex;
		}
	}
}