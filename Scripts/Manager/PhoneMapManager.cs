using Halabang.Plugin;
using UnityEngine;
using UnityEngine.UI;

namespace Halabang.Blueberry.pp {
  /// <summary>
  /// 地图appManager
  /// </summary>

  public class PhoneMapManager : MonoBehaviour {
    public PhoneMapController _PhoneMapController => mapController;
    public float _dragSensitivity => dragSensitivity;
    public float _minValue => minValue;
    public float _maxValue => maxValue;


    [Header("拖拽设置")]
    [SerializeField] PhoneMapController mapController;
    [Tooltip("拖拽灵敏度，值越大拖动越灵敏")]
    [SerializeField] private float dragSensitivity = 1f;
    [Header("进度条参数")]
    [SerializeField] public float progressDuration = 5f;
    [SerializeField] private float minValue = 0f; // 进度条最小值
    [SerializeField] private float maxValue = 100f; // 进度条最大值

    //获取打开的controller面板
    public GameObject GetPanelStatus() {
      if (_PhoneMapController._panelStatus == true) {
        return _PhoneMapController.gameObject;
      }

      return null;
    }
    //关闭打开的面板
    public void Close() {
      if (GetPanelStatus() != null)
        _PhoneMapController.CloseApp();
    }
    public void Open() {
      _PhoneMapController.OpenApp();
    }
  }
}