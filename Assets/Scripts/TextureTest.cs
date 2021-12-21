using System;
using System.Collections;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Rect = UnityEngine.Rect;
[Obsolete]
public class TextureTest : MonoBehaviour
{
    public Texture2D Txture2d;
    private Texture2D img=null;
    public Image image;
    public Sprite sprite;

    public void Start()
    {
    }

    public void SelectImg()
    {
        IOTool.Instance.OpenNativeImage(image);
    }
    public void SaveImg()
    {
        IOTool.Instance.SaveImage(View.Instance.CurrentNode);
    }
    IEnumerator GetTexture(string url){
        UnityWebRequest www = new UnityWebRequest(url);
        //img = www.downloadHandler.data.;
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        www.downloadHandler = texDl;
        yield return www.SendWebRequest();
        if (www.isDone && www.error == null)
        {
            sprite = Sprite.Create(texDl.texture, new Rect(0, 0, texDl.texture.width, texDl.texture.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
        }
    }

    public void LineCheck()
    {
        Texture2D imgTexture = View.Instance.CurrentNode.GetTexture2D();
        Mat imgMat = new Mat (imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat (imgTexture, imgMat);
        Debug.Log ("imgMat.ToString() " + imgMat.ToString ());
        Mat grayMat = new Mat ();
        Imgproc.cvtColor (imgMat, grayMat, Imgproc.COLOR_RGB2GRAY);
        Imgproc.Canny(grayMat, grayMat, 50, 200);
        Texture2D texture = new Texture2D (grayMat.cols (), grayMat.rows (), TextureFormat.RGBA32, false);
        Utils.matToTexture2D (grayMat, texture);
        var rSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        View.Instance.CurrentNode.sprite = rSprite;
        View.Instance.CurrentImg.sprite = View.Instance.CurrentNode.sprite;
        View.Instance.CurrentNode.SpriteStacker.Push(rSprite);
        //img = texture;
    }
    public void ImageRuiHua()
    {
        Texture2D imgTexture = img;
        Mat imgMat = new Mat (imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat (imgTexture, imgMat);
        Debug.Log ("imgMat.ToString() " + imgMat.ToString ());
        Mat kernel = new Mat(3, 3, CvType.CV_16SC1);
        kernel.put(0, 0, 0, -1, 0, -1, 5, -1, 0, -1, 0);
        Imgproc.filter2D(imgMat, imgMat, imgMat.depth(), kernel);
        Texture2D texture = new Texture2D (imgMat.cols (), imgMat.rows (), TextureFormat.RGBA32, false);
        Utils.matToTexture2D (imgMat, texture);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
        img = texture;
    }

    public void ImagePlus()
    {
        string rExplorePath = EditorUtility.OpenFilePanel("选择图片", "C:/Users/gy/Desktop", "*");
        if (!string.IsNullOrEmpty(rExplorePath))
        {
            StartCoroutine(GetTexture(rExplorePath));
        }
    }
}
