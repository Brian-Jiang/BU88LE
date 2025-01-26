using UnityEngine;
using System.IO;
using System.Collections;

public class ScreenshotCapture : MonoBehaviour
{
    public string screenshotFolder = "Screenshots"; // 保存截图的文件夹

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CaptureScreenshot());
        }
    }

    IEnumerator CaptureScreenshot()
    {
        // 等待一帧，确保画面渲染完成
        yield return new WaitForEndOfFrame();

        // 确保保存截图的文件夹存在
        string folderPath = Path.Combine(Application.dataPath, screenshotFolder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 生成唯一的文件名（带时间戳）
        string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string fullPath = Path.Combine(folderPath, filename);

        // 截取屏幕并保存
        ScreenCapture.CaptureScreenshot(fullPath);

        Debug.Log("截图已保存: " + fullPath);
    }
}
