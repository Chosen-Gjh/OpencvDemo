using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 程序入口
/// </summary>
public class Entry : MonoBehaviour
{
    public Image Mid;

    public GameObject Item;

    public Transform Content;

    public GameObject Button;

    public Transform ButtonContent;

    public ImgProcessFactory ImgProcessFatory;
    // Start is called before the first frame update
    void Start()
    {
        IOTool.Instance.Initialize();
        View.Instance.Initialize(this.Mid,this.Item,this.Content);
        
        //拿到类
        var IPFType = ImgProcessFactory.Instance.GetType();
        
        var Functions = IPFType.GetMethods(BindingFlags.Default|BindingFlags.Instance|BindingFlags.Public|BindingFlags.Static);
        var IPEntry = IPFType.GetMethod("ImgProcess2");
        for (int i = 0; i < Functions.Length; i++)
        {
            if (Functions[i].GetCustomAttributes(typeof(ProcessFuncAttribute),true).Any())
            {
                var NewButton = GameObject.Instantiate(this.Button, this.ButtonContent);
                NewButton.GetComponentInChildren<Text>().text = Functions[i].Name;
                var rProcess = IPFType.GetMethod(Functions[i].Name);//Functions[i];
                NewButton.GetComponent<Button>().onClick.AddListener(delegate()
                {
                    IPEntry.Invoke(this,new object[]
                    {
                        View.Instance.CurrentNode,rProcess
                    });
                });
            }
        }
    }
    
    public static T ToEnum<T>(string name)
    {
        return (T)Enum.Parse(typeof(T), name);
    }
}
