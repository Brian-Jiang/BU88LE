using Graphics;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public new Camera camera;
    public BubbleRenderManager bubbleRenderManager;
    public MeshRenderer refImageRenderer;
    
    private Material refImageMaterialInstance;
    
    public void SetRefImage(Texture2D texture)
    {
        refImageMaterialInstance.SetTexture("_BaseMap", texture);
    }

    private void Start()
    {
        camera.orthographicSize = bubbleRenderManager.transform.localScale.y / 2 / camera.aspect;
        refImageMaterialInstance = refImageRenderer.material;
    }
}