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
    public ShowSimilarity showSimilarity;
    
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
        
        if (showSimilarity.CalculateSimilarity(similarity))
        {
            audioManager.Play("pop");
        }
        else
        {
            audioManager.Play("lose");
        }

        // showSimilarity.CalculateSimilarity(bubbleRenderManager.CompareWithReference());
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
        
        bubbleSpawner.OnAddBubble += OnAddBubble;
        bubbleSpawner.OnRemoveBubble += OnRemoveBubble;
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
        for (int i = 0; i < bubbleList.Count; i++)
        {
            var screenPos = camera.WorldToViewportPoint(bubbleList[i].center);
            screenPos.y = (screenPos.y - 0.5f) / camera.aspect + 0.5f;
            bubbleRenderManager.SetBubblePosition(i, screenPos);
            bubbleRenderManager.SetBubbleSize(i, bubbleList[i].radius);
        }
    }
    
    private void OnAddBubble()
    {
        // lastSize = bubbleSpawner.BubbleList.Count;
        bubbleRenderManager.AddBubble();
    }
    
    private void OnRemoveBubble(int index)
    {
        // if (bubbleSpawner.BubbleList.Count < lastSize)
        // {
        //     lastSize = bubbleSpawner.BubbleList.Count;
        //     audioManager.Play("pop");
        // }
        bubbleRenderManager.RemoveBubble(index);
    }
}