using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnSpheres : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public GameObject Bubble;
    public Camera MainCamera;
    public float GrowthSpeed = 2f;
    public float MinRadius = 1f;
    public float MaxRadius = 5f;
    public float MergeSpeed = 2f;
    public float MergeThreshold = 0.1f;
    public List<(Vector3 center, float radius)> BubbleList = new();
    public List<List<int>> BubbleMergeList = new();
    private List<GameObject> bubbles = new();
    
    void Start()
    {
        if(MainCamera == null)
            MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        BubbleMerge();
        
        // new bubble
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = MainCamera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0; 
            if (FindBubble(mousePos)==-1)
            {
                GameObject newCircle = Instantiate(Bubble, mousePos, quaternion.identity);
                newCircle.transform.localScale = Vector3.one * (MinRadius * 2);
                BubbleList.Add((mousePos, MinRadius));
                Debug.Log("Spawn Spheres:"+mousePos+" "+MinRadius);
            }
        }
        
        // Bubble grows
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = MainCamera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0; 

            int currCenterIndex = FindBubble(mousePos);
            if (currCenterIndex != -1 && BubbleList[currCenterIndex].radius < MaxRadius)
            {
                var bubble = BubbleList[currCenterIndex];
                bubble.radius += GrowthSpeed * Time.deltaTime;
                BubbleList[currCenterIndex] = bubble;
                //Debug.Log("Radius:"+bubble.radius);
                
                FindBubbleObject(currCenterIndex);
            }
            
        }
    }

    // Check if the mouse position is inside the bubble
    int FindBubble(Vector3 MouseLocation)
    {
        for(int i=0; i<BubbleList.Count; i++)
        {
            if (Vector3.Distance(BubbleList[i].center, MouseLocation) <= BubbleList[i].radius)
            {
                Debug.Log("Distance:"+Vector3.Distance(BubbleList[i].center, MouseLocation));
                return i;
            }
        }
        return -1;
    }
    
    // Find the bubble gameobject according to the center of the circle
    void FindBubbleObject(int BubbleIndex)
    {
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("Bubble");
        foreach (GameObject bubble in bubbles)
        {
            if (Vector3.Distance(bubble.transform.position, BubbleList[BubbleIndex].center) <= 0.1)
            { 
                bubble.transform.localScale = Vector3.one * BubbleList[BubbleIndex].radius * 2;
                bubble.transform.position = BubbleList[BubbleIndex].center;
            }
        }
    }
    
    void BubbleMerge()
    {
        for (int i = 0; i < BubbleList.Count; i++)
        {
            for (int j = i + 1; j < BubbleList.Count; j++)
            {
                float distance = Vector3.Distance(BubbleList[i].center, BubbleList[j].center);
                float radiusSum = BubbleList[i].radius + BubbleList[j].radius;
                
                if (distance <= radiusSum)
                {
                    // check if bubble i and bubble j in BubbleMergeList
                    int ConMerged = 0;
                    int MergeHead = 0;
                    int MergeGroupIdx = 0;
                    for (MergeGroupIdx = 0; MergeGroupIdx < BubbleMergeList.Count; MergeGroupIdx++)
                    {
                        if (BubbleMergeList[MergeGroupIdx].Contains(i))
                        {
                            BubbleMergeList[MergeGroupIdx].Add(j);
                            ConMerged = 1;
                            MergeHead = BubbleMergeList[MergeGroupIdx][0];
                            break;
                        }
                        else if(BubbleMergeList[MergeGroupIdx].Contains(j))
                        {
                            BubbleMergeList[MergeGroupIdx].Add(i);
                            ConMerged = 2;
                            MergeHead = BubbleMergeList[MergeGroupIdx][0];
                            break;
                        }
                    }
                    if (ConMerged==0)
                    {
                        BubbleMergeList.Add(new List<int> { i, j });
                        MergeGroupIdx = BubbleMergeList.Count - 1;
                    }

                    // Merge to first bubble in the Group
                    if (ConMerged==1)
                    {
                        Vector3 direction = (BubbleList[MergeHead].center - BubbleList[j].center).normalized;
                        BubbleList[j] = (BubbleList[j].center - direction * MergeSpeed * (BubbleList[j].radius / radiusSum), BubbleList[j].radius);
                    }
                    else if (ConMerged == 2)
                    {
                        Vector3 direction = (BubbleList[MergeHead].center - BubbleList[i].center).normalized;
                        BubbleList[i] = (BubbleList[i].center + direction * MergeSpeed * (BubbleList[i].radius / radiusSum), BubbleList[i].radius);
                    }
                    else
                    {
                        // Move bubble based on radius ratio
                        Vector3 direction = (BubbleList[j].center - BubbleList[i].center).normalized;
                        BubbleList[i] = (BubbleList[i].center + direction * MergeSpeed * (BubbleList[i].radius / radiusSum), BubbleList[i].radius);
                        BubbleList[j] = (BubbleList[j].center - direction * MergeSpeed * (BubbleList[j].radius / radiusSum), BubbleList[j].radius);
                    }

                    MergeStop(MergeGroupIdx, i, j);
                    FindBubbleObject(i);
                    FindBubbleObject(j);
                }
            }
        }
    }

    void MergeStop(int MergeGroupIdx, int bubble1, int bubble2)
    {
        if (Vector3.Distance(BubbleList[bubble1].center, BubbleList[bubble2].center) <= MergeThreshold)
        {
            BubbleMergeList[MergeGroupIdx].Remove(bubble2);
            BubbleList.RemoveAt(bubble2);
            if (BubbleMergeList[MergeGroupIdx].Count < 2)
            {
                BubbleMergeList.RemoveAt(MergeGroupIdx);
            }
        }
    }
}
