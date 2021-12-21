using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Model 单例层
/// </summary>
public class Model
{
    public Image CurrentImg;
    
    public List<Image> ImgList;

    public List<ImgNode> ImgNodeList; 
    
    private static Model _instance;
    
    public static Model Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Model();
                _instance.Start();
            }

            return _instance;
        }
    }
    
    public void Start()
    {
        ImgList = new List<Image>();
        ImgNodeList = new List<ImgNode>();
    }

}
