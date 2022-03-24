using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyShader : MonoBehaviour
{
    public Material material; // Wraps the shader
    public RenderTexture texture;
    private RenderTexture buffer;

    public Texture initialTexture;

    private float lastUpdateTime = 0;
    public float updateInterval = 0.1f; // Seconds

    void Start()
    {
        Graphics.Blit(initialTexture, texture);
        buffer = new RenderTexture(texture.width, texture.height, texture.depth, texture.format);
    }

    public void UpdateTexture()
    {
        Graphics.Blit(texture, buffer, material);
        Graphics.Blit(buffer, texture);
    }

    public void Update()
    {
        if (Time.time > lastUpdateTime + updateInterval)
        {
            UpdateTexture();
            lastUpdateTime = Time.time;
        }
    }
}
