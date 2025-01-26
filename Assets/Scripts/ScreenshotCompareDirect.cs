using UnityEngine;
using System.IO;
using System.Collections;

public class ScreenshotCompareDirect : MonoBehaviour
{
    public GameObject quad;  // 需要截图的 Quad 对象
    public string referenceImagePath = "ReferenceScreenshots/Level1.png"; // 关卡参考图片路径（相对路径）

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CaptureAndCompare());
        }
    }

    IEnumerator CaptureAndCompare()
    {
        yield return new WaitForEndOfFrame();

        // 截图并缩放
        //Texture2D screenshot = CaptureScreenshotToTexture();
        //Texture2D screenshot = CaptureQuadScreenshot(quad, 512, 512);
        //Texture2D screenshot = Capture3DView(quad, 512, 512);
        //Texture2D screenshot = CaptureQuadScreenshot(quad);
        Texture2D screenshot = Capture3DView(quad);
        Texture2D resizedScreenshot = ResizeTexture(screenshot, 50, 50);

        byte[] bytes = resizedScreenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/screenshot.png", bytes);
        Debug.Log("Quad Screenshot saved!");

        // 加载参考图片并缩放
        string fullReferencePath = Path.Combine(Application.dataPath, referenceImagePath);
        if (!File.Exists(fullReferencePath))
        {
            Debug.LogError("找不到参考截图: " + fullReferencePath);
            yield break;
        }

        Texture2D referenceImage = LoadTexture(fullReferencePath);
        Texture2D resizedReference = ResizeTexture(referenceImage, 50, 50);

        // 进行相似度比较
        float similarity = CompareTextures(resizedScreenshot, resizedReference);
        float similarityAndSaveDiff = CompareTexturesAndSaveDiff(resizedScreenshot, resizedReference, Application.dataPath + "/Screenshots/diff.png");
        Debug.Log($"当前关卡与参考图片的相似度: {similarity * 100:F2}%");

        // 释放纹理
        Destroy(screenshot);
        Destroy(resizedScreenshot);
        Destroy(referenceImage);
        Destroy(resizedReference);
    }

    Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        RenderTexture.active = rt;

        UnityEngine.Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    #region 弃用的截图方法

    //Texture2D CaptureScreenshotToTexture()
    //{
    //    // 截取屏幕尺寸
    //    int width = Screen.width;
    //    int height = Screen.height;

    //    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    screenshot.Apply();

    //    Debug.Log("已截图并存入内存");
    //    return screenshot;
    //}

    //public Texture2D CaptureQuadScreenshot(GameObject quad, int width, int height)
    //{


    //    // 1. 创建 RenderTexture
    //    RenderTexture rt = new RenderTexture(width, height, 24);

    //    // 2. 获取 Quad 上的材质和主贴图
    //    Renderer quadRenderer = quad.GetComponent<Renderer>();
    //    Material quadMaterial = quadRenderer.material;
    //    Texture quadTexture = quadMaterial.mainTexture;

    //    // 3. 将材质上的贴图渲染到 RenderTexture
    //    UnityEngine.Graphics.Blit(quadTexture, rt);

    //    // 4. 读取 RenderTexture 数据并转换为 Texture2D
    //    RenderTexture.active = rt;
    //    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    screenshot.Apply();

    //    // 5. 清理 RenderTexture
    //    RenderTexture.active = null;
    //    rt.Release();

    //    return screenshot;
    //}

    //public Texture2D Capture3DView(GameObject targetObject, int width, int height)
    //{


    //    // 1. 创建 RenderTexture
    //    RenderTexture rt = new RenderTexture(width, height, 24);

    //    // 2. 创建一个新的临时摄像机
    //    GameObject tempCamObj = new GameObject("TempCamera");
    //    Camera tempCam = tempCamObj.AddComponent<Camera>();

    //    // 3. 设置摄像机的参数
    //    tempCam.transform.position = targetObject.transform.position + targetObject.transform.forward * -5f; // 设置视角距离
    //    tempCam.transform.LookAt(targetObject.transform);  // 摄像机朝向目标物体
    //    tempCam.targetTexture = rt;
    //    tempCam.clearFlags = CameraClearFlags.SolidColor;
    //    tempCam.backgroundColor = Color.clear;
    //    tempCam.orthographic = false;  // 透视投影

    //    // 4. 渲染目标对象
    //    tempCam.Render();

    //    // 5. 读取 RenderTexture 数据并转换为 Texture2D
    //    RenderTexture.active = rt;
    //    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    screenshot.Apply();

    //    // 6. 清理资源
    //    RenderTexture.active = null;
    //    tempCam.targetTexture = null;
    //    GameObject.Destroy(tempCamObj);
    //    rt.Release();

    //    return screenshot;
    //}

    //public Texture2D CaptureQuadScreenshot(GameObject quad)
    //{
    //    // 获取 Quad 的尺寸（基于局部缩放）
    //    Renderer quadRenderer = quad.GetComponent<Renderer>();
    //    int width = Mathf.RoundToInt(quadRenderer.bounds.size.x * 100);  // 转换为像素单位
    //    int height = Mathf.RoundToInt(quadRenderer.bounds.size.y * 100);

    //    // 创建 RenderTexture
    //    RenderTexture rt = new RenderTexture(width, height, 24);

    //    // 获取 Quad 的材质和主贴图
    //    Material quadMaterial = quadRenderer.material;
    //    Texture quadTexture = quadMaterial.mainTexture;

    //    // 渲染到 RenderTexture
    //    UnityEngine.Graphics.Blit(quadTexture, rt);

    //    // 读取 RenderTexture 数据并转换为 Texture2D
    //    RenderTexture.active = rt;
    //    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    screenshot.Apply();

    //    // 清理资源
    //    RenderTexture.active = null;
    //    rt.Release();

    //    return screenshot;
    //}

    #endregion

    public Texture2D Capture3DView(GameObject targetObject)
    {
        // 1. 获取 Quad 尺寸
        Renderer quadRenderer = targetObject.GetComponent<Renderer>();
        Vector3 quadSize = quadRenderer.bounds.size;
        int width = Mathf.RoundToInt(quadSize.x * 100);  // 根据大小调整像素分辨率
        int height = Mathf.RoundToInt(quadSize.y * 100);

        // 2. 创建 RenderTexture
        RenderTexture rt = new RenderTexture(width, height, 24);

        // 3. 创建临时摄像机
        GameObject tempCamObj = new GameObject("TempCamera");
        Camera tempCam = tempCamObj.AddComponent<Camera>();

        // 4. 计算摄像机位置和设置参数
        float distance = Mathf.Max(quadSize.x, quadSize.y) * 2.0f;  // 根据大小调整距离
        tempCam.transform.position = targetObject.transform.position + targetObject.transform.forward * -distance;
        tempCam.transform.LookAt(targetObject.transform.position);

        // 设置摄像机视口大小，使 Quad 刚好填满
        tempCam.aspect = (float)width / height;
        tempCam.orthographic = false;  // 透视投影，保持 3D 效果
        tempCam.fieldOfView = 30f;  // 适当的 FOV 调整可以适应 Quad 大小

        tempCam.targetTexture = rt;
        tempCam.clearFlags = CameraClearFlags.SolidColor;
        tempCam.backgroundColor = Color.clear;

        // 5. 渲染目标对象
        tempCam.Render();

        // 6. 读取 RenderTexture 数据并转换为 Texture2D
        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // 7. 清理资源
        RenderTexture.active = null;
        tempCam.targetTexture = null;
        GameObject.Destroy(tempCamObj);
        rt.Release();

        return screenshot;
    }


    Texture2D LoadTexture(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);  // 默认大小，会被图片数据覆盖
        texture.LoadImage(fileData);  // 加载 PNG/JPG 文件
        return texture;
    }

    float CompareTextures(Texture2D tex1, Texture2D tex2)
    {
        if (tex1.width != tex2.width || tex1.height != tex2.height)
        {
            Debug.LogError("图片尺寸不一致，无法比较");
            return 0f;
        }

        Color32[] pixels1 = tex1.GetPixels32();
        Color32[] pixels2 = tex2.GetPixels32();
        int totalPixels = pixels1.Length;
        int matchingPixels = 0;

        for (int i = 0; i < totalPixels; i++)
        {
            if (ColorDifference(pixels1[i], pixels2[i]) < 0.1f)  // 误差阈值
            {
                matchingPixels++;
            }
        }

        return (float)matchingPixels / totalPixels;
    }

    public float CompareTexturesAndSaveDiff(Texture2D tex1, Texture2D tex2, string savePath)
    {
        if (tex1.width != tex2.width || tex1.height != tex2.height)
        {
            Debug.LogError("图片尺寸不一致，无法比较");
            return 0f;
        }

        Color32[] pixels1 = tex1.GetPixels32();
        Color32[] pixels2 = tex2.GetPixels32();
        Color32[] diffPixels = new Color32[pixels1.Length];

        int totalPixels = pixels1.Length;
        int matchingPixels = 0;
        float threshold = 0.1f; // 误差阈值

        for (int i = 0; i < totalPixels; i++)
        {
            if (ColorDifference(pixels1[i], pixels2[i]) < threshold)
            {
                matchingPixels++;
                diffPixels[i] = new Color32(0, 0, 0, 0);  // 透明，表示相同
            }
            else
            {
                diffPixels[i] = new Color32(255, 0, 0, 255);  // 红色，表示不同
            }
        }

        float similarity = (float)matchingPixels / totalPixels;

        // 创建差异图
        Texture2D diffTexture = new Texture2D(tex1.width, tex1.height);
        diffTexture.SetPixels32(diffPixels);
        diffTexture.Apply();

        // 保存差异图为 PNG
        byte[] pngData = diffTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, pngData);

        Debug.Log($"差异图已保存至: {savePath}");
        return similarity;
    }


    float ColorDifference(Color32 a, Color32 b)
    {
        float rDiff = Mathf.Abs(a.r - b.r) / 255f;
        float gDiff = Mathf.Abs(a.g - b.g) / 255f;
        float bDiff = Mathf.Abs(a.b - b.b) / 255f;
        return (rDiff + gDiff + bDiff) / 3f;
    }
}