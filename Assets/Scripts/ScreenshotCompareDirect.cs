using UnityEngine;
using System.IO;
using System.Collections;

public class ScreenshotCompareDirect : MonoBehaviour
{
    public GameObject quad;  // ��Ҫ��ͼ�� Quad ����
    public string referenceImagePath = "ReferenceScreenshots/Level1.png"; // �ؿ��ο�ͼƬ·�������·����

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

        // ��ͼ������
        //Texture2D screenshot = CaptureScreenshotToTexture();
        //Texture2D screenshot = CaptureQuadScreenshot(quad, 512, 512);
        //Texture2D screenshot = Capture3DView(quad, 512, 512);
        //Texture2D screenshot = CaptureQuadScreenshot(quad);
        Texture2D screenshot = Capture3DView(quad);
        Texture2D resizedScreenshot = ResizeTexture(screenshot, 50, 50);

        byte[] bytes = resizedScreenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/screenshot.png", bytes);
        Debug.Log("Quad Screenshot saved!");

        // ���زο�ͼƬ������
        string fullReferencePath = Path.Combine(Application.dataPath, referenceImagePath);
        if (!File.Exists(fullReferencePath))
        {
            Debug.LogError("�Ҳ����ο���ͼ: " + fullReferencePath);
            yield break;
        }

        Texture2D referenceImage = LoadTexture(fullReferencePath);
        Texture2D resizedReference = ResizeTexture(referenceImage, 50, 50);

        // �������ƶȱȽ�
        float similarity = CompareTextures(resizedScreenshot, resizedReference);
        float similarityAndSaveDiff = CompareTexturesAndSaveDiff(resizedScreenshot, resizedReference, Application.dataPath + "/Screenshots/diff.png");
        Debug.Log($"��ǰ�ؿ���ο�ͼƬ�����ƶ�: {similarity * 100:F2}%");

        // �ͷ�����
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

    #region ���õĽ�ͼ����

    //Texture2D CaptureScreenshotToTexture()
    //{
    //    // ��ȡ��Ļ�ߴ�
    //    int width = Screen.width;
    //    int height = Screen.height;

    //    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    screenshot.Apply();

    //    Debug.Log("�ѽ�ͼ�������ڴ�");
    //    return screenshot;
    //}

    //public Texture2D CaptureQuadScreenshot(GameObject quad, int width, int height)
    //{


    //    // 1. ���� RenderTexture
    //    RenderTexture rt = new RenderTexture(width, height, 24);

    //    // 2. ��ȡ Quad �ϵĲ��ʺ�����ͼ
    //    Renderer quadRenderer = quad.GetComponent<Renderer>();
    //    Material quadMaterial = quadRenderer.material;
    //    Texture quadTexture = quadMaterial.mainTexture;

    //    // 3. �������ϵ���ͼ��Ⱦ�� RenderTexture
    //    UnityEngine.Graphics.Blit(quadTexture, rt);

    //    // 4. ��ȡ RenderTexture ���ݲ�ת��Ϊ Texture2D
    //    RenderTexture.active = rt;
    //    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    screenshot.Apply();

    //    // 5. ���� RenderTexture
    //    RenderTexture.active = null;
    //    rt.Release();

    //    return screenshot;
    //}

    //public Texture2D Capture3DView(GameObject targetObject, int width, int height)
    //{


    //    // 1. ���� RenderTexture
    //    RenderTexture rt = new RenderTexture(width, height, 24);

    //    // 2. ����һ���µ���ʱ�����
    //    GameObject tempCamObj = new GameObject("TempCamera");
    //    Camera tempCam = tempCamObj.AddComponent<Camera>();

    //    // 3. ����������Ĳ���
    //    tempCam.transform.position = targetObject.transform.position + targetObject.transform.forward * -5f; // �����ӽǾ���
    //    tempCam.transform.LookAt(targetObject.transform);  // ���������Ŀ������
    //    tempCam.targetTexture = rt;
    //    tempCam.clearFlags = CameraClearFlags.SolidColor;
    //    tempCam.backgroundColor = Color.clear;
    //    tempCam.orthographic = false;  // ͸��ͶӰ

    //    // 4. ��ȾĿ�����
    //    tempCam.Render();

    //    // 5. ��ȡ RenderTexture ���ݲ�ת��Ϊ Texture2D
    //    RenderTexture.active = rt;
    //    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    screenshot.Apply();

    //    // 6. ������Դ
    //    RenderTexture.active = null;
    //    tempCam.targetTexture = null;
    //    GameObject.Destroy(tempCamObj);
    //    rt.Release();

    //    return screenshot;
    //}

    //public Texture2D CaptureQuadScreenshot(GameObject quad)
    //{
    //    // ��ȡ Quad �ĳߴ磨���ھֲ����ţ�
    //    Renderer quadRenderer = quad.GetComponent<Renderer>();
    //    int width = Mathf.RoundToInt(quadRenderer.bounds.size.x * 100);  // ת��Ϊ���ص�λ
    //    int height = Mathf.RoundToInt(quadRenderer.bounds.size.y * 100);

    //    // ���� RenderTexture
    //    RenderTexture rt = new RenderTexture(width, height, 24);

    //    // ��ȡ Quad �Ĳ��ʺ�����ͼ
    //    Material quadMaterial = quadRenderer.material;
    //    Texture quadTexture = quadMaterial.mainTexture;

    //    // ��Ⱦ�� RenderTexture
    //    UnityEngine.Graphics.Blit(quadTexture, rt);

    //    // ��ȡ RenderTexture ���ݲ�ת��Ϊ Texture2D
    //    RenderTexture.active = rt;
    //    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    //    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    screenshot.Apply();

    //    // ������Դ
    //    RenderTexture.active = null;
    //    rt.Release();

    //    return screenshot;
    //}

    #endregion

    public Texture2D Capture3DView(GameObject targetObject)
    {
        // 1. ��ȡ Quad �ߴ�
        Renderer quadRenderer = targetObject.GetComponent<Renderer>();
        Vector3 quadSize = quadRenderer.bounds.size;
        int width = Mathf.RoundToInt(quadSize.x * 100);  // ���ݴ�С�������طֱ���
        int height = Mathf.RoundToInt(quadSize.y * 100);

        // 2. ���� RenderTexture
        RenderTexture rt = new RenderTexture(width, height, 24);

        // 3. ������ʱ�����
        GameObject tempCamObj = new GameObject("TempCamera");
        Camera tempCam = tempCamObj.AddComponent<Camera>();

        // 4. ���������λ�ú����ò���
        float distance = Mathf.Max(quadSize.x, quadSize.y) * 2.0f;  // ���ݴ�С��������
        tempCam.transform.position = targetObject.transform.position + targetObject.transform.forward * -distance;
        tempCam.transform.LookAt(targetObject.transform.position);

        // ����������ӿڴ�С��ʹ Quad �պ�����
        tempCam.aspect = (float)width / height;
        tempCam.orthographic = false;  // ͸��ͶӰ������ 3D Ч��
        tempCam.fieldOfView = 30f;  // �ʵ��� FOV ����������Ӧ Quad ��С

        tempCam.targetTexture = rt;
        tempCam.clearFlags = CameraClearFlags.SolidColor;
        tempCam.backgroundColor = Color.clear;

        // 5. ��ȾĿ�����
        tempCam.Render();

        // 6. ��ȡ RenderTexture ���ݲ�ת��Ϊ Texture2D
        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // 7. ������Դ
        RenderTexture.active = null;
        tempCam.targetTexture = null;
        GameObject.Destroy(tempCamObj);
        rt.Release();

        return screenshot;
    }


    Texture2D LoadTexture(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);  // Ĭ�ϴ�С���ᱻͼƬ���ݸ���
        texture.LoadImage(fileData);  // ���� PNG/JPG �ļ�
        return texture;
    }

    float CompareTextures(Texture2D tex1, Texture2D tex2)
    {
        if (tex1.width != tex2.width || tex1.height != tex2.height)
        {
            Debug.LogError("ͼƬ�ߴ粻һ�£��޷��Ƚ�");
            return 0f;
        }

        Color32[] pixels1 = tex1.GetPixels32();
        Color32[] pixels2 = tex2.GetPixels32();
        int totalPixels = pixels1.Length;
        int matchingPixels = 0;

        for (int i = 0; i < totalPixels; i++)
        {
            if (ColorDifference(pixels1[i], pixels2[i]) < 0.1f)  // �����ֵ
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
            Debug.LogError("ͼƬ�ߴ粻һ�£��޷��Ƚ�");
            return 0f;
        }

        Color32[] pixels1 = tex1.GetPixels32();
        Color32[] pixels2 = tex2.GetPixels32();
        Color32[] diffPixels = new Color32[pixels1.Length];

        int totalPixels = pixels1.Length;
        int matchingPixels = 0;
        float threshold = 0.1f; // �����ֵ

        for (int i = 0; i < totalPixels; i++)
        {
            if (ColorDifference(pixels1[i], pixels2[i]) < threshold)
            {
                matchingPixels++;
                diffPixels[i] = new Color32(0, 0, 0, 0);  // ͸������ʾ��ͬ
            }
            else
            {
                diffPixels[i] = new Color32(255, 0, 0, 255);  // ��ɫ����ʾ��ͬ
            }
        }

        float similarity = (float)matchingPixels / totalPixels;

        // ��������ͼ
        Texture2D diffTexture = new Texture2D(tex1.width, tex1.height);
        diffTexture.SetPixels32(diffPixels);
        diffTexture.Apply();

        // �������ͼΪ PNG
        byte[] pngData = diffTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, pngData);

        Debug.Log($"����ͼ�ѱ�����: {savePath}");
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