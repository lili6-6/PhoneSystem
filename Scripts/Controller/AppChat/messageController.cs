using Halabang.Plugin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// 文本消息controller。
    /// </summary>
    public class messageController : MonoBehaviour
    {
        [SerializeField] private Image Picture;
        [SerializeField] private TextMeshExtend Message;
        public Image _Picture=>Picture;
        public TextMeshExtend _Message =>Message;
        public string nameStr { get; private set; }
        public void changeNameStr(string str)
        {
            nameStr = str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="picture">头像</param>
        /// <param name="message">消息内容</param>
        /// <param name="nameStr">发送人</param>
        public void setContent(Sprite picture,string message,string nameStr)
        {
            Picture.sprite = picture;
            Message.SetText(message);
            this.nameStr = nameStr;
        }
    }
}