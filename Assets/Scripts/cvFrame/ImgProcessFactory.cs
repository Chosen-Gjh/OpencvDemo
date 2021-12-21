using System;
using System.Reflection;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine;
using Rect = UnityEngine.Rect;
[Obsolete]
public enum ProcessType
{
    None = 0,
    Canny = 1,
    RuiHua
}
public class ImgProcessFactory
{
    private static ImgProcessFactory _instance;
    
    public static ImgProcessFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ImgProcessFactory();
            }

            return _instance;
        }
    }

    [Obsolete]
    public static void ImgProcess(ProcessType rProType,ImgNode rTargetNode,Func<Sprite,Sprite> rProcess)
    {
        if (rTargetNode == null)
            return;
        Sprite rResSprite = rProcess(rTargetNode.sprite);
        //处理
        switch (rProType)
        {
            case ProcessType.None:
                return;
            case ProcessType.Canny:
                rResSprite = Canny(rTargetNode.ReturnSprite());
                break;
            case ProcessType.RuiHua:
                //rResSprite = RuiHua(rTargetNode.ReturnSprite());
                break;
            default:
                return;
        }
        rTargetNode.sprite = rResSprite;
        //保存
        rTargetNode.SpriteStacker.Push(rResSprite);
        //显示
        View.Instance.ViewFresh();
    }
    public static void ImgProcess2(ImgNode rTargetNode,MethodInfo rProcess)//Func<Sprite,Sprite> rProcess)
    {
        if (rTargetNode == null)
            return;
        //Sprite rsp = new Sprite();
        rTargetNode.sprite=(Sprite)rProcess.Invoke(null, new []{rTargetNode.sprite});
        //处理
        //保存
        rTargetNode.SpriteStacker.Push(rTargetNode.sprite);
        //显示
        View.Instance.ViewFresh();
    }

    #region Facede 模式

    [ProcessFunc]
    public static Sprite Canny(Sprite rSprite)
    {
        var imgTexture = rSprite.texture;        
        Mat imgMat = new Mat (imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat (imgTexture, imgMat);
        Mat grayMat = new Mat ();
        Imgproc.cvtColor (imgMat, grayMat, Imgproc.COLOR_RGB2GRAY);
        Imgproc.Canny(grayMat, grayMat, 50, 200);
        Texture2D texture = new Texture2D (grayMat.cols (), grayMat.rows (), TextureFormat.RGBA32, false);
        Utils.matToTexture2D (grayMat, texture);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    
    [ProcessFunc]
    public static Sprite Sharpen(Sprite rSprite)
    {
        Texture2D imgTexture = rSprite.texture;
        Mat imgMat = new Mat (imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat (imgTexture, imgMat);
        Mat kernel = new Mat(3, 3, CvType.CV_16SC1);
        kernel.put(0, 0, 0, -1, 0, -1, 5, -1, 0, -1, 0);
        Imgproc.filter2D(imgMat, imgMat, imgMat.depth(), kernel);
        Texture2D texture = new Texture2D (imgMat.cols (), imgMat.rows (), TextureFormat.RGBA32, false);
        Utils.matToTexture2D (imgMat, texture);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    [ProcessFunc]
    public static Sprite GuassBlur(Sprite rSprite)
    {
        Texture2D imgTexture = rSprite.texture;
        Mat imgMat = new Mat (imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat (imgTexture, imgMat);
        Imgproc.GaussianBlur (imgMat, imgMat, new Size (3, 3), 2, 2);
        Texture2D texture = new Texture2D (imgMat.cols (), imgMat.rows (), TextureFormat.RGBA32, false);
        Utils.matToTexture2D (imgMat, texture);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    
    [ProcessFunc]
    public static Sprite Comic(Sprite rSprite)
    {
        Texture2D imgTexture = rSprite.texture;
        Mat imgMat = new Mat (imgTexture.height, imgTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat (imgTexture, imgMat);
        Imgproc.GaussianBlur (imgMat, imgMat, new Size (3, 3), 2, 2);
        Texture2D texture = new Texture2D (imgMat.cols (), imgMat.rows (), TextureFormat.RGBA32, false);
        Utils.matToTexture2D (imgMat, texture);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    #endregion

}
