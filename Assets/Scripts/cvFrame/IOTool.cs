using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using Rect = UnityEngine.Rect;
using UnityEngine.UI;
using UnityEngine.Networking;
/// <summary>
/// 工具单例类
/// </summary>
public class IOTool
{
    private static IOTool _instance;

    private CoroutineMono CoroutineRoot;
    public static IOTool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IOTool();
            }

            return _instance;
        }
    }

    public void Initialize()
    {
        GameObject CoroutineRootObj = new GameObject("CoroutineRootObj");
        this.CoroutineRoot = CoroutineRootObj.AddComponent<CoroutineMono>();
    }
   

    IEnumerator GetTexture(string url, Texture2D tx2, Image image)
    {
        //添加Img
        UnityWebRequest www = new UnityWebRequest(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        www.downloadHandler = texDl;
        yield return www.SendWebRequest();
        if (www.isDone && www.error == null)
        {
            var sprite = Sprite.Create(texDl.texture, new Rect(0, 0, texDl.texture.width, texDl.texture.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
            
            //节点创造
            var rImgNode = new ImgNode()
            {
                sprite = sprite
            };
            rImgNode.SpriteStacker.Push(sprite);
            Model.Instance.ImgNodeList.Add(rImgNode);
            //拓展右边的图片选项 显示到当前的图片
            View.Instance.Add(rImgNode);
            rImgNode.ImgNodeShow += delegate (Texture2D img)
            {
                
            };
        }

    }

    public Texture2D OpenNativeImage(Image TargetImg)
    {
        string rExplorePath = EditorUtility.OpenFilePanel("选择图片", "C:/Users/gy/Desktop", "*");
        Texture2D tx2 = null;
        if (!string.IsNullOrEmpty(rExplorePath))
        {
            CoroutineRoot.StartCoroutine(GetTexture(rExplorePath,tx2,TargetImg));
        }
        return tx2;
    }

    public void SaveImage(ImgNode rNode)
    {
        var path = EditorUtility.SaveFilePanel("Save Preset", $"{DateTime.Now.ToString().Replace(":","-").Replace("/","-")}.png", "png", "");
        if (!string.IsNullOrEmpty(path))
        {
            var rBytes = rNode.sprite.texture.EncodeToPNG();
            File.WriteAllBytes(path,rBytes);
        }  
    }
}
