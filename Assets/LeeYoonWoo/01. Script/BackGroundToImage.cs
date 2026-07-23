using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WallpaperLoader : MonoBehaviour
{
    public RawImage rawImage;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool SystemParametersInfo(
        int action,
        int param,
        StringBuilder path,
        int flags
    );

    private void Start()
    {
        StringBuilder path = new StringBuilder(260);

        // 현재 바탕화면 경로 가져오기
        SystemParametersInfo(0x0073, path.Capacity, path, 0);

        byte[] data = File.ReadAllBytes(path.ToString());

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(data);

        rawImage.texture = texture;
    }
}