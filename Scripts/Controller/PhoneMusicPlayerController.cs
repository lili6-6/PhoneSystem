using UnityEngine;

namespace Halabang.Blueberry.pp
{

    /// <summary>
    /// 音乐app的controller。
    /// </summary>

    public class PhoneMusicPlayerController : PhoneAppController
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private PhoneMusicPlayerManager phoneMusicPlayerManager;

        public override void Start()
        {
            base.Start();
            phoneMusicPlayerManager = BlueberryManager.Instance.CurrentPhoneManager._PhoneMusicPlayerManager;

        }

    }
}