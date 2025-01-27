using TMPro;
using UnityEngine;

public class ShowSimilarity : MonoBehaviour
{

    public GameObject winUI;
    public GameObject loseUI;
    public float threshold;

    private TextMeshProUGUI similarityText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        similarityText = GetComponent<TextMeshProUGUI>();
    }


    public bool CalculateSimilarity(float similarity)
    {
        similarity *= 100;
        similarityText.text = "Similarity: " + similarity.ToString("F2") + " %";
        if (similarity > threshold)
        {
            winUI.SetActive(true);
            loseUI.SetActive(false);
            return true;
        }
        else
        {
            winUI.SetActive(false);
            loseUI.SetActive(true);
            return false;
        }
    }

    
}
