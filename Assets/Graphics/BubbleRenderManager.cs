﻿using System.Collections.Generic;
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