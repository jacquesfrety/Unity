using UnityEngine;
using UnityEngine.Rendering;

public class ExportHDRI : MonoBehaviour
{
    public int faceSize = 2048;
    public int width = 4096;   // height = width / 2

    [ContextMenu("Export HDRI")]
    void Export()
    {
        var cam = new GameObject("tmpCam").AddComponent<Camera>();
        cam.transform.position = transform.position;
        cam.allowHDR = true;
        cam.clearFlags = CameraClearFlags.Skybox;

        var cube = new RenderTexture(faceSize, faceSize, 24, RenderTextureFormat.ARGBHalf)
        { dimension = TextureDimension.Cube };
        var equi = new RenderTexture(width, width / 2, 0, RenderTextureFormat.ARGBHalf);

        cam.RenderToCubemap(cube);
        cube.ConvertToEquirect(equi);

        var prev = RenderTexture.active;
        RenderTexture.active = equi;
        var tex = new Texture2D(width, width / 2, TextureFormat.RGBAHalf, false, true);
        tex.ReadPixels(new Rect(0, 0, width, width / 2), 0, 0);
        tex.Apply();
        RenderTexture.active = prev;

        System.IO.File.WriteAllBytes(Application.dataPath + "/scene_hdri.exr",
            tex.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat));

        DestroyImmediate(cam.gameObject);
    }
}