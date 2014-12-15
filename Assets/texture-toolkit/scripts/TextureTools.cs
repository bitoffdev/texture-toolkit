using UnityEngine;
//using System.Collections;

namespace texturetk
{
	public class TextureTools
	{
		/// <summary>
		/// Draws a line from pos1 to pos2 on the given texture
		/// </summary>
		public static void DrawLine (Texture2D tex, Vector2 pos1, Vector2 pos2, Color col){
			if (pos2.x-pos1.x == 0f){//Check if vertical line
				for (int y=(int)Mathf.Min (pos1.y, pos2.y); y<(int)Mathf.Max (pos1.y, pos2.y); y++){
					tex.SetPixel((int)pos1.x, y, col);
				}
			} else {
				float m = (pos2.y - pos1.y) / (pos2.x - pos1.x);//Line slope
				float b = -m * pos1.x + pos1.y;//Y-intercept
				int blockH = (int)Mathf.Abs(m) + 1;
				Color[] cols = new Color[blockH];
				for (int i=0; i<cols.Length; i++) {cols[i] = col;}
				for (float x=Mathf.Min(pos1.x, pos2.x); x<Mathf.Max(pos1.x, pos2.x); x++) {//Iterate through domain of segment
					tex.SetPixels((int)x, (int)(m*x+b), 1, blockH, cols);
				}
			}
			tex.Apply ();
		}
		public static void DrawRect (Texture2D tex, Vector2 pos1, Vector2 pos2, Color col){
			Vector2 start = new Vector2(Mathf.Clamp(Mathf.Min(pos1.x, pos2.x), 0, tex.width), Mathf.Clamp (Mathf.Min(pos1.y, pos2.y), 0, tex.width));
			Vector2 end = new Vector2(Mathf.Clamp(Mathf.Max(pos1.x, pos2.x), 0, tex.width), Mathf.Clamp (Mathf.Max(pos1.y, pos2.y), 0, tex.width));
			for (int x=(int)start.x;x<=(int)end.x;x++){
				for (int y=(int)start.y;y<=(int)end.y;y++){
					tex.SetPixel(x,y,col);
				}
			}
			tex.Apply();
			return;
		}
		/// <summary>
		/// Flips the given texture vertically
		/// </summary>
		public static void FlipY(Texture2D tex){
			Color[] pix = new Color[tex.width*tex.height];
			for (int x=0;x<tex.width;x++){
				for (int y=0;y<tex.height;y++){
					pix[x + y * tex.width] = tex.GetPixel(x, tex.height - y);
				}
			}
			tex.SetPixels (pix);
			tex.Apply ();
		}
		/// <summary>
		/// Rotates the given texture 90 degrees
		/// </summary>
		public static void Rotate(Texture2D tex){
			Color[] pix = new Color[tex.width*tex.height];
			for (int x=0;x<tex.width;x++){
				for (int y=0;y<tex.height;y++){
					pix[x + y * tex.width] = tex.GetPixel(tex.height - y, x);
				}
			}
			tex.SetPixels (pix);
			tex.Apply ();
		}
		/// <summary>
		/// Converts Grayscale Textures to Colored Texture
		/// </summary>
		public static void GrayscaleToColor(Texture2D tex, Color start, Color end)
		{
			Color[] pix = tex.GetPixels ();
			for (int i=0; i<pix.Length; i++) {
				pix[i] = Color.Lerp (start, end, pix[i].grayscale);
			}
			tex.SetPixels (pix);
			tex.Apply ();
		}
	}
}