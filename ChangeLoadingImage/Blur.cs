using UnityEngine;
using System.Collections;

namespace ChangeLoadingImage
{
    //http://forum.unity3d.com/threads/contribution-texture2d-blur-in-c.185694/#post-1637841
    public class Blur : MonoBehaviour
    {
        
        public int radius = 2;
        public int iterations = 2;

        public static Texture2D FastBlur(Texture2D image, int radius, int iterations)
        {
            Texture2D tex = image;

            for (var i = 0; i < iterations; i++)
            {
                tex = BlurImage(tex, radius);
            }

            return tex;
        }

        private static void GenerateGaussianKernel(int radius, out float[] kernel)
        {
            if (radius <= 0)
            {
                kernel = new float[] { 1.0f };
                return;
            }

            int kernelSize = (radius * 2) + 1;
            kernel = new float[kernelSize];
            float sigma = (float)radius; // Per instructions, radius is sigma
            float sum = 0f;

            float twoSigmaSq = 2 * sigma * sigma;
            float sqrtTwoPiSigmaSq = Mathf.Sqrt(2 * Mathf.PI * sigma * sigma);

            if (sqrtTwoPiSigmaSq == 0) // Should be caught by radius <= 0, but as a safeguard
            {
                kernel = new float[] { 1.0f };
                return;
            }

            for (int i = 0; i < kernelSize; i++)
            {
                int x = i - radius;
                kernel[i] = (1.0f / sqrtTwoPiSigmaSq) * Mathf.Exp(-(x * x) / twoSigmaSq);
                sum += kernel[i];
            }

            // Normalize the kernel
            if (sum == 0 || float.IsNaN(sum) || float.IsInfinity(sum))
            {
                // Fallback if sum is zero, NaN, or Inf (e.g. if sigma was tiny and all kernel values underflowed or overflowed)
                // Create a kernel that does nothing (center pixel gets 1.0 weight).
                for(int i=0; i<kernelSize; ++i) kernel[i] = 0f;
                if (kernelSize > 0) kernel[kernelSize/2] = 1.0f; // kernelRadius is kernelSize/2
                return;
            }
            
            for (int i = 0; i < kernelSize; i++)
            {
                kernel[i] /= sum;
            }
        }

        static Texture2D BlurImage(Texture2D image, int blurRadius)
        {
            int _W = image.width;
            int _H = image.height;
            Texture2D blurred = new Texture2D(_W, _H, image.format, false);
            Texture2D tempTexture = new Texture2D(_W, _H, image.format, false); // For intermediate horizontal blur

            float[] kernel;
            GenerateGaussianKernel(blurRadius, out kernel);
            int kernelRadius = kernel.Length / 2;

            // Horizontal Pass
            for (int yy = 0; yy < _H; yy++)
            {
                for (int xx = 0; xx < _W; xx++)
                {
                    float r = 0f, g = 0f, b = 0f;
                    for (int k = -kernelRadius; k <= kernelRadius; k++)
                    {
                        int x = Mathf.Clamp(xx + k, 0, _W - 1);
                        Color pixel = image.GetPixel(x, yy);
                        float weight = kernel[k + kernelRadius];
                        r += pixel.r * weight;
                        g += pixel.g * weight;
                        b += pixel.b * weight;
                    }
                    tempTexture.SetPixel(xx, yy, new Color(r, g, b, 1.0f));
                }
            }
            tempTexture.Apply();

            // Vertical Pass (operates on tempTexture)
            for (int xx = 0; xx < _W; xx++)
            {
                for (int yy = 0; yy < _H; yy++)
                {
                    float r = 0f, g = 0f, b = 0f;
                    for (int k = -kernelRadius; k <= kernelRadius; k++)
                    {
                        int y = Mathf.Clamp(yy + k, 0, _H - 1);
                        Color pixel = tempTexture.GetPixel(xx, y); // Read from horizontally blurred texture
                        float weight = kernel[k + kernelRadius];
                        r += pixel.r * weight;
                        g += pixel.g * weight;
                        b += pixel.b * weight;
                    }
                    blurred.SetPixel(xx, yy, new Color(r, g, b, 1.0f));
                }
            }
            blurred.Apply();
            
            // Cleanup intermediate texture
            if (Application.isPlaying) {
                Destroy(tempTexture);
            } else {
                DestroyImmediate(tempTexture);
            }

            return blurred;
        }
    }
}