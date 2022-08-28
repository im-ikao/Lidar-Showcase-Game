using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PaintableSurface : MonoBehaviour
{
    private const float COLOR_IMPACT_ON_PIXEL_DRAW = 0.25f;
    private const int TEXTURE_SIZE_PER_UNIT = 96;

    [SerializeField] private Material _paintableMaterial;
    [SerializeField] private Renderer _objectRenderer;
    [SerializeField] private Color _emissionColor;
    
    private Texture _texture;
    private Texture2D _texture2D;

    private void Awake() 
    {
        _objectRenderer.material = _paintableMaterial;

        _texture2D = new Texture2D(
            TEXTURE_SIZE_PER_UNIT * (int)transform.lossyScale.x,
            TEXTURE_SIZE_PER_UNIT * (int)transform.lossyScale.y);
        
        NullifyTexture();
        
        _objectRenderer.material.SetTexture("_PaintedTexture", _texture2D);
        _objectRenderer.material.SetColor("_EmissionColor", _emissionColor);
    }

    public void DrawPixelOnRaycastHit(RaycastHit hit) 
    {
        var pixelUV = hit.textureCoord;
        var pixelPoint = new Vector2(pixelUV.x * _texture2D.width, pixelUV.y * _texture2D.height);
        var tiling = _objectRenderer.material.GetTextureScale("_PaintedTexture");
        
        _texture2D.SetPixel((int) (pixelPoint.x * tiling.x), (int) (pixelPoint.y * tiling.y), Color.white);
    }

    public void ApplyTextureChanges() 
    {
        _texture2D.Apply();
    }

    private void NullifyTexture() 
    {
        for (var i = 0; i < _texture2D.width; i++) 
        {
            for (var j = 0; j < _texture2D.height; j++)
            {
                _texture2D.SetPixel(i, j, Color.black);
            }
        }

        _texture2D.Apply();
    }
}
