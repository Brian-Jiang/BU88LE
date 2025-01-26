using Unity.Mathematics;
using UnityEngine;

public class SpawnSpheres : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public GameObject sphere;
    public Camera mainCamera;
    
    void Start()
    {
        if(mainCamera == null)
            mainCamera = Camera.main;
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(sphere, hit.point, quaternion.identity);
            }
        }
            
    }
}
