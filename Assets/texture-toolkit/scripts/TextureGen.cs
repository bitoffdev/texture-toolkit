using UnityEngine;
using System.Collections;

namespace texturetk
{
	public class TextureGen
	{
		/// <summary>Procedural Marble Texture</summary>
		/// <param name="w">Pixel width of texture</param>
		/// <param name="h">Pixel height of texture</param>
		/// <param name="xPeriod">defines repetition of marble lines in x direction (Default 5)</param>
		/// <param name="yPeriod">defines repetition of marble lines in y direction (Default 10)</param>
		/// <param name="turbPower">Makes twists (Default 0.1)</param>
		/// <param name="turbSize">Initial size of the turbulence (Default 32)</param>
		/// <remarks>
		/// xPeriod and yPeriod together define the angle of the lines
		/// xPeriod and yPeriod both 0 ==> it becomes a normal clouds or turbulence pattern
		/// turbPower = 0 ==> it becomes a normal sine pattern
		/// </remarks>
		/// <returns>Returns a Texture2D</returns>
		public static Texture2D marble(int w = 512, int h = 512, float xPeriod = 5f, float yPeriod = 10f, float turbPower = 300f, float turbSize = 32f)
		{
			Color[] pix = new Color[w * h];
			
			for(int x = 0; x < w; x++){
				for(int y = 0; y < h; y++){   
					int i = x + y * w;
					float xyValue = x * xPeriod / h + y * yPeriod / w + turbPower * turbulence(x, y, turbSize) / 256f;
					float sineValue = 256f * Mathf.Abs(Mathf.Sin(xyValue * 3.14159f));
					pix[i] = Color.Lerp (Color.black, Color.white, sineValue/255);
				}
			}
			
			Texture2D noiseTex = new Texture2D (w, h);
			noiseTex.SetPixels(pix);
			noiseTex.Apply();
			return noiseTex;
		}
		/// <summary>Procedural Wood Texture</summary>
		/// <param name="w">Pixel width of texture</param>
		/// <param name="h">Pixel height of texture</param>
		/// <param name="xyPeriod">number of rings (Default 12)</param>
		/// <param name="turbPower">Makes twists (Default 0.1)</param>
		/// <param name="turbSize">Initial size of the turbulence (Default 32)</param>
		/// <returns>Returns a Texture2D</returns>
		public static Texture2D wood(int w = 512, int h = 512, float xyPeriod = 12f, float turbPower = 0.1f, float turbSize = 32f)
		{
			Color[] pix = new Color[w * h];
			
			for(int x = 0; x < w; x++){
				for(int y = 0; y < h; y++){   
					int i = x + y * w;
					float xValue = (x - h / 2f) / (float)h;
					float yValue = (y - w / 2f) / (float)w;
					float distValue = Mathf.Sqrt(xValue * xValue + yValue * yValue) + turbPower * turbulence(x, y, turbSize) / 256f;
					float sineValue = 128f * Mathf.Abs(Mathf.Sin(2f * xyPeriod * distValue * 3.14159f));
					pix[i] = Color.Lerp (Color.black, Color.white, sineValue/255);
				}
			}

			Texture2D noiseTex = new Texture2D (w, h);
			noiseTex.SetPixels(pix);
			noiseTex.Apply();
			return noiseTex;
		}
		/// <summary>Procedural Cloud Texture</summary>
		/// <param name="w">Pixel width of texture</param>
		/// <param name="h">Pixel height of texture</param>
		/// <param name="size">Lower values are grittier, higher values are smoother</param>
		/// <returns>Returns a Texture2D</returns>
		public static Texture2D clouds(int w = 512, int h = 512, float size = 64f)
		{
			Color[] pix = new Color[w * h];
			
			for(int x = 0; x < w; x++){
				for(int y = 0; y < h; y++){   
					int i = x + y * w;
					float val = turbulence(x, y, size);
					pix[i] = Color.Lerp (Color.black, Color.white, val);
				}
			}
			
			Texture2D noiseTex = new Texture2D (w, h);
			noiseTex.SetPixels(pix);
			noiseTex.Apply();
			return noiseTex;
		}
		/// <summary>Procedural XOR (tiled) Texture</summary>
		/// <param name="w">Pixel width of texture</param>
		/// <param name="h">Pixel height of texture</param>
		/// <remarks>The texture will look best when the w and h parameters are powers of two.</remarks>
		/// <returns>Returns a Texture2D</returns>
		public static Texture2D xor(int w = 512, int h = 512)
		{
			Color[] pix = new Color[w * h];
			float size = Mathf.Max (w, h);
			
			for(int x = 0; x < w; x++){
				for(int y = 0; y < h; y++){   
					int i = x + y * w;
					float val = (x ^ y)/size;
					pix[i] = Color.Lerp (Color.black, Color.white, val);
				}
			}
			
			Texture2D noiseTex = new Texture2D (w, h);
			noiseTex.SetPixels(pix);
			noiseTex.Apply();
			return noiseTex;
		}
		/// <summary>
		/// Turbulence given the specified x, y and size.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="size">Size.</param>
		static float turbulence(float x, float y, float size){
			float value = 0.0f;
			float n = size;
			
			while(n >= 1) {
				value += Mathf.PerlinNoise(x / n, y / n) * n;
				n /= 2.0f;
			}
			
			return(value / size);
		}
	}
}