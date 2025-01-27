using System.Collections.Generic;
using UnityEngine;

namespace Graphics
{
    public class BubbleRenderManager : MonoBehaviour
    {
        public MeshRenderer bubbleRenderer;
        
        public Vector4[] bubblePositions;
        public float[] bubbleSizes;
        public float[] bubbleRotationSpeeds;
        public int bubbleCount;
        
        private Material bubbleMaterialInstance;
        
        private static readonly int BubbleSizesProp = Shader.PropertyToID("_BubbleSizes");
        private static readonly int BubblePositionsProp = Shader.PropertyToID("_BubblePositions");
        private static readonly int BubbleCountProp = Shader.PropertyToID("_BubbleCount");
        
        public const int MAX_BUBBLE_COUNT = 64;
        
        public void ClearAllBubbles()
        {
            bubbleCount = 0;
        }
        
        public int AddBubble(Vector3 position, float size, float rotationSpeed)
        {
            if (bubbleCount >= MAX_BUBBLE_COUNT)
            {
                Debug.LogWarning("Bubble count exceeded maximum limit");
                return -1;
            }
            
            bubblePositions[bubbleCount] = new Vector4(position.x, position.y, position.z, 0);
            bubbleSizes[bubbleCount] = size;
            bubbleRotationSpeeds[bubbleCount] = rotationSpeed;
            bubbleCount++;
            
            return bubbleCount - 1;
        }

        public void AddBubble()
        {
            if (bubbleCount >= MAX_BUBBLE_COUNT)
            {
                Debug.LogWarning("Bubble count exceeded maximum limit");
                return;
            }
            
            bubbleCount++;
        }
        
        public void SetBubblePosition(int index, Vector3 position)
        {
            if (index < 0 || index >= bubbleCount)
            {
                Debug.LogWarning("Invalid bubble index");
                return;
            }
            
            bubblePositions[index] = new Vector4(position.x, position.y, position.z, 0);
        }
        
        public void SetBubbleSize(int index, float size)
        {
            if (index < 0 || index >= bubbleCount)
            {
                Debug.LogWarning("Invalid bubble index");
                return;
            }
            
            bubbleSizes[index] = size;
        }
        
        public void SetBubbleRotationSpeed(int index, float rotationSpeed)
        {
            if (index < 0 || index >= bubbleCount)
            {
                Debug.LogWarning("Invalid bubble index");
                return;
            }
            
            bubbleRotationSpeeds[index] = rotationSpeed;
        }
        
        public void RemoveBubble(int index)
        {
            if (index < 0 || index >= bubbleCount)
            {
                Debug.LogWarning("Invalid bubble index");
                return;
            }
            
            for (int i = index; i < bubbleCount - 1; i++)
            {
                bubblePositions[i] = bubblePositions[i + 1];
                bubbleSizes[i] = bubbleSizes[i + 1];
                bubbleRotationSpeeds[i] = bubbleRotationSpeeds[i + 1];
            }
            
            bubbleCount--;
        }
        
        public void Snapshot()
        {
            for (int i = 0; i < bubbleCount; i++)
            {
                bubbleRotationSpeeds[i] = 0;
            }
        }

        private void Start()
        {
            bubblePositions = new Vector4[MAX_BUBBLE_COUNT];
            bubbleSizes = new float[MAX_BUBBLE_COUNT];
            bubbleRotationSpeeds = new float[MAX_BUBBLE_COUNT];
            
            bubbleMaterialInstance = bubbleRenderer.material;
            
            bubblePositions[0] = new Vector4(0.9f, 0.72f, 0, 0);
            bubblePositions[1] = new Vector4(0.58f, 0.72f, 0, 0);
            bubbleSizes[0] = 0.2f;
            bubbleSizes[1] = 0.26f;
            bubbleRotationSpeeds[0] = 0.5f;
            bubbleRotationSpeeds[1] = -0.3f;
            bubbleCount = 2;
        }

        private void Update()
        {
            bubbleMaterialInstance.SetInt(BubbleCountProp, bubbleCount);
            if (bubbleCount > 0)
            {
                bubbleMaterialInstance.SetVectorArray(BubblePositionsProp, bubblePositions);
                bubbleMaterialInstance.SetFloatArray(BubbleSizesProp, bubbleSizes);
                bubbleMaterialInstance.SetFloatArray("_BubbleRotationSpeeds", bubbleRotationSpeeds);
            }
        }
    }
}