using UnityEngine;

public class BlurEffect : MonoBehaviour
{
    public Material blurMaterial; // The material with the blur shader
    public int iterations = 4;    // Number of blur iterations
    public float blurSize = 1.0f; // Blur size
    public float randomness = 0.5f; // Randomness for the blur effect
    public float opacity = 1.0f;   // Opacity of the effect
    public int randomSeed = 0;     // Random seed for randomness

    private void Start()
    {
        // Set the random seed for consistent randomness
        Random.InitState(randomSeed);
    }

    // Call this method to apply the blur effect
    public void ApplyBlurEffect(RenderTexture targetTexture)
    {
        // Set the shader properties
        blurMaterial.SetFloat("_BlurSize", blurSize);
        blurMaterial.SetFloat("_Randomness", randomness);
        blurMaterial.SetFloat("_Opacity", opacity);

        // Apply blur effect for the specified number of iterations
        for (int i = 0; i < iterations; i++)
        {
            // You can apply the effect multiple times to create stronger blur
            Graphics.Blit(targetTexture, targetTexture, blurMaterial);
        }
    }

    // To apply this effect on a camera's render texture (or use in other ways)
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        ApplyBlurEffect(src);
        Graphics.Blit(src, dest);
    }
}
