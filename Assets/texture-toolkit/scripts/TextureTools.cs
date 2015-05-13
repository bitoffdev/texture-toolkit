using UnityEngine;
using System.IO;

namespace texturetk
{
	public class TextureTools
	{
		/// <summary>
		/// Draws a circle on the given texture
		/// </summary>
		public static void DrawCircle(Texture2D tex, Vector2 pos, int radius, Color col){
			DrawCircle (tex, (int)pos.x, (int)pos.y, radius, col);
		}
		public static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color col)
		{
			int x, y, px, nx, py, ny, d;
			
			for (x = 0; x <= r; x++)
			{
				d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
				for (y = 0; y <= d; y++)
				{
					px = cx + x;
					nx = cx - x;
					py = cy + y;
					ny = cy - y;
					
					tex.SetPixel(px, py, col);
					tex.SetPixel(nx, py, col);
					
					tex.SetPixel(px, ny, col);
					tex.SetPixel(nx, ny, col);
					
				}
			}    
			tex.Apply ();
		}
		/// <summary>
		/// Draws a line from pos1 to pos2 on the given texture
		/// </summary>
		public static void DrawLine (Texture2D tex, Vector2 pos1, Vector2 pos2, Color col){
			DrawLine (tex, (int)pos1.x, (int)pos1.y, (int)pos2.x, (int)pos2.y, col);
		}
		public static void DrawLine (Texture2D tex, int x1, int y1, int x2, int y2, Color col){
			if (x2-x1 == 0f){//Check if vertical line
				for (int y=(int)Mathf.Min (y1, y2); y<(int)Mathf.Max (y1, y2); y++){
					tex.SetPixel((int)x1, y, col);
				}
			} else {
				float m = (y2 - y1) / (x2 - x1);//Line slope
				float b = -m * x1 + y1;//Y-intercept
				int blockH = (int)Mathf.Abs(m) + 1;
				Color[] cols = new Color[blockH];
				for (int i=0; i<cols.Length; i++) {cols[i] = col;}
				for (float x=Mathf.Min(x1, x2); x<Mathf.Max(x1, x2); x++) {//Iterate through domain of segment
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
		/// <summary>
		/// Saves a texture at the given path
		/// </summary>
		public static void SaveTexture(Texture2D tex, string path)
		{
			if (!string.IsNullOrEmpty(path)){
				byte[] bytes = tex.EncodeToPNG();
				File.WriteAllBytes(path, bytes);
			}
		}
	}
}