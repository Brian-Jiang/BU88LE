using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnSpheres : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    // public GameObject Bubble;
    public Camera MainCamera;
    public float GrowthSpeed = 2f;
    public float MinRadius = 1f;
    public float MaxRadius = 5f;
    public float MergeSpeed = 2f;
    public float MergeThreshold = 0.1f;
    public List<(Vector3 center, float radius)> BubbleList = new();
    public List<List<int>> BubbleMergeList = new();
    // private List<GameObject> BubbleObjects = new();
    
    public event Action OnAddBubble;
    public event Action<int> OnRemoveBubble; 
    
    void Start()
    {
        if(MainCamera == null)
            MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        // new bubble
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = MainCamera.ScreenToWorldPoint(mousePos);
            mousePos.z = 0; 
            if (FindBubble(mousePos)==-1)
            {
                // GameObject newCircle = Instantiate(Bubble, mousePos, quaternion.identity);
                // newCircle.transform.localScale = Vector3.one * (MinRadius * 2);
                // BubbleObjects.Add(newCircle);
                BubbleList.Add((mousePos, MinRadius));
                OnAddBubble?.Invoke();
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
                
                // FindBubbleObject(currCenterIndex);
            }
            
        }
        BubbleMerge();
        for(int i=0; i<BubbleMergeList.Count; i++)
            MergeGroup(i);
            
        // for(int i=0; i<BubbleList.Count; i++)
            // FindBubbleObject(i);
            
        // Debug.Log("MergeGroup Number:"+BubbleMergeList.Count);
        // Debug.Log("BubbleList count:"+BubbleList.Count);

        // if (BubbleMergeList.Count > 0)
        // {
        //    foreach(var item in BubbleMergeList[0])
        //        Debug.Log("MergeList unit:"+item); 
        // }

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
    // void FindBubbleObject(int BubbleIndex)
    // {
    //     BubbleObjects[BubbleIndex].transform.localScale = Vector3.one * BubbleList[BubbleIndex].radius * 2;
    //     BubbleObjects[BubbleIndex].transform.position = BubbleList[BubbleIndex].center;
    // }
    
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
                    bool ConMerged = true;
                    int MergeGroupIdx;
                    int GroupI=-1, GroupJ=-1;
                    for (MergeGroupIdx = 0; MergeGroupIdx < BubbleMergeList.Count; MergeGroupIdx++)
                    {
                        if (BubbleMergeList[MergeGroupIdx].Contains(i))
                        {
                            GroupI = MergeGroupIdx;
                        }
                        
                        if(BubbleMergeList[MergeGroupIdx].Contains(j))
                        {
                            GroupJ = MergeGroupIdx;
                        }
                    }

                    if (GroupI != -1 && GroupJ == -1)
                    {
                        if (BubbleList[BubbleMergeList[GroupI][0]].radius < BubbleList[i].radius)
                        {
                            BubbleMergeList[GroupI].Add(BubbleMergeList[GroupI][0]);
                            BubbleMergeList[GroupI][0] = j;
                        }
                        else
                        {
                            BubbleMergeList[GroupI].Add(j);
                        }
                        Debug.Log("Add to GroupI");
                    }
                    else if (GroupI == -1 && GroupJ != -1)
                    {
                        if (BubbleList[BubbleMergeList[GroupJ][0]].radius < BubbleList[j].radius)
                        {
                            BubbleMergeList[GroupJ].Add(BubbleMergeList[GroupJ][0]);
                            BubbleMergeList[GroupJ][0] = i;
                        }
                        else
                        {
                            BubbleMergeList[GroupJ].Add(i);
                        }
                        Debug.Log("Add to GroupJ");
                    }
                    else if (GroupI == -1 && GroupJ == -1)
                    {
                        if(BubbleList[i].radius > BubbleList[j].radius)
                            BubbleMergeList.Add(new List<int> { i, j });
                        else
                            BubbleMergeList.Add(new List<int> { i, j });
                    }
                }
            }
        }
    }

    void MergeGroup(int MergeGroupIdx)
    {
        if (MergeGroupIdx >= BubbleMergeList.Count || BubbleMergeList[MergeGroupIdx].Count < 2)
        {
            return;
        }
        
        for (int i = BubbleMergeList[MergeGroupIdx].Count - 1; i >= 1; i--)
        {
            Debug.Log("MergeList count:"+BubbleMergeList[MergeGroupIdx].Count);
            if (BubbleMergeList[MergeGroupIdx][0] >= BubbleList.Count || BubbleMergeList[MergeGroupIdx][i] >= BubbleList.Count)
            {
                continue;
            }
            Vector3 direction = (BubbleList[BubbleMergeList[MergeGroupIdx][0]].center - BubbleList[BubbleMergeList[MergeGroupIdx][i]].center).normalized;
            float radiusSum = BubbleList[BubbleMergeList[MergeGroupIdx][0]].radius + BubbleList[BubbleMergeList[MergeGroupIdx][i]].radius;
            BubbleList[BubbleMergeList[MergeGroupIdx][i]] = (BubbleList[BubbleMergeList[MergeGroupIdx][i]].center + direction * MergeSpeed * (BubbleList[BubbleMergeList[MergeGroupIdx][i]].radius / radiusSum)*Time.deltaTime, BubbleList[BubbleMergeList[MergeGroupIdx][i]].radius);
            MergeStop(MergeGroupIdx, BubbleMergeList[MergeGroupIdx][0], BubbleMergeList[MergeGroupIdx][i]);
        }
    }
    
    void MergeStop(int MergeGroupIdx, int bubble1, int bubble2)
    {
        if (Vector3.Distance(BubbleList[bubble1].center, BubbleList[bubble2].center) <= MergeThreshold)
        {
            BubbleMergeList[MergeGroupIdx].Remove(bubble2);
            BubbleList.RemoveAt(bubble2);
            OnRemoveBubble?.Invoke(bubble2);
            // Destroy(BubbleObjects[bubble2]);
            // BubbleObjects.RemoveAt(bubble2);
            if (BubbleMergeList[MergeGroupIdx].Count < 2)
            {
                BubbleMergeList.RemoveAt(MergeGroupIdx);
            }
        }
    }
}
