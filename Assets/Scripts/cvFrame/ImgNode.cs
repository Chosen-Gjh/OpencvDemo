using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void Show(Texture2D img);
/// <summary>
/// 图像节点信息类
/// </summary>
public class ImgNode
{
    public Sprite sprite;
    public event Show ImgNodeShow;
    public Stack<Sprite> SpriteStacker = new Stack<Sprite>();
    public Sprite ReturnSprite()
    {
        return sprite;
    }

    public Texture2D GetTexture2D()
    {
        return sprite.texture;
    }

    public Sprite Revoke()
    {
        if (SpriteStacker!=null&&SpriteStacker.Count >1)
        {
            this.SpriteStacker.Pop();
            this.sprite = SpriteStacker.Peek();
        }
        return this.sprite;
    }
}
