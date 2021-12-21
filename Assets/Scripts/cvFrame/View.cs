using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 视图层 
/// </summary>
public class View
{
    public Image CurrentImg;
    public int CurrentShowImgIndex;
    public ImgNode CurrentNode;
    public List<GameObject> ImgList;
    public GameObject PreViewItem;
    public Transform Content;
    private static View _instance;

    public static View Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new View();
            }

            return _instance;
        }
    }

    public void Initialize(Image rTargetImg, GameObject rPreViewItem,Transform rContent)
    {
        CurrentImg = rTargetImg;
        ImgList = new List<GameObject>();
        this.PreViewItem = rPreViewItem;
        this.Content = rContent;
    }

    public void Add(ImgNode rNode)
    {
        var rSprite = rNode.sprite;
        if (ImgList != null)
        {
            var NewNode = GameObject.Instantiate(this.PreViewItem, this.Content);
            var Child = NewNode.transform.Find("Image");
            Child.GetComponent<Image>().sprite = rSprite;
            var RevokeBtn = NewNode.transform.Find("Revoke");
            RevokeBtn.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                rNode.Revoke();
                ShowImg(rNode);
            });
            Adapter(Child.GetComponent<RectTransform>());
            //添加点击事件
            NewNode.GetComponent<Button>().onClick.AddListener(delegate () {
                ShowImg(rNode);
            });
            CurrentNode = rNode;
        }
    }

    private void Adapter(RectTransform rTrans)
    {
        var rSprite = rTrans.GetComponent<Image>().sprite;
        if(rSprite.rect.width> rSprite.rect.height)
        {
            rTrans.sizeDelta = new Vector2(200,(float)rSprite.rect.height/(float)rSprite.rect.width * 200f);
        }
        else
        {
            rTrans.sizeDelta = new Vector2((float)rSprite.rect.width/(float)rSprite.rect.height * 200f,200);
        }
    }

    public void Remove(GameObject rTarget)
    {
        if(ImgList!=null)
            ImgList.Remove(rTarget);
    }
    
    public void RemoveAt(int imgIndex)
    {
        if(ImgList!=null&&imgIndex<ImgList.Count)
            ImgList.RemoveAt(imgIndex);
    }

    public void ShowImg(ImgNode rNode)
    {
        CurrentImg.sprite = rNode.sprite;
        CurrentNode = rNode;
    }

    public void ViewFresh()
    {
        this.CurrentImg.sprite = this.CurrentNode.sprite;
    }
}
