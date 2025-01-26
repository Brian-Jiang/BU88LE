using Graphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public new Camera camera;
    public BubbleRenderManager bubbleRenderManager;
    public MeshRenderer refImageRenderer;
    public AudioManager audioManager;
    
    private Material refImageMaterialInstance;
    
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
    }
}