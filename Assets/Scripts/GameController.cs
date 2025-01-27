using Graphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public new Camera camera;
    public BubbleRenderManager bubbleRenderManager;
    public MeshRenderer refImageRenderer;
    public AudioManager audioManager;
    public ScreenshotCompareDirect screenshotCompareDirect;
    public SpawnSpheres bubbleSpawner;
    
    private Material refImageMaterialInstance;
    private int lastSize = 0;
    
    public void SetRefImage(Texture2D texture)
    {
        refImageMaterialInstance.SetTexture("_BaseMap", texture);
    }
    
    public void ClearAllBubbles()
    {
        bubbleRenderManager.ClearAllBubbles();
    }
    
    public void Snapshot()
    {
        // bubbleRenderManager.Snapshot();
        audioManager.Play("snapshot");
        var similarity = screenshotCompareDirect.CaptureAndCompare("ReferenceScreenshots/Level1.png");
        
    }

    void ReloadLevel()
    {
        // Reload the current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Level reloaded!");
    }

    private void Start()
    {
        camera.orthographicSize = bubbleRenderManager.transform.localScale.y / 2 / camera.aspect;
        refImageMaterialInstance = refImageRenderer.material;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Snapshot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }
        
        var bubbleList = bubbleSpawner.BubbleList;
        
    }
}