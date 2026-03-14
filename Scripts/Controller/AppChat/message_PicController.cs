using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 图片消息controller。存储图片消息
    /// </summary>
    public class message_PicController : MonoBehaviour
    {
        [SerializeField] private Image Picture;
        [SerializeField] private Image Message_Pic;
        public Image _Picture=>Picture;
        public Image _Message_Pic =>Message_Pic;
        public string NameStr {  get; private set; }//发送人
        public void changeNameStr(string str)
        {
            NameStr = str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="picture">头像</param>
        /// <param name="message_Pic">图片消息</param>
        /// <param name="nameStr">发送人</param>
        public void setContent(Sprite picture,Sprite message_Pic,string nameStr)
        {
            Picture.sprite = picture;
            Message_Pic.sprite = message_Pic;
            NameStr = nameStr;
        }
    }
}