using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Halabang.Plugin;
using Halabang.UI;
using System.Collections.Generic;
using Halabang.Game;
using System.Linq;

namespace Halabang.Blueberry.pp {
  [RequireComponent(typeof(RectTransform))]
  //继承IPointerDownHandler, IPointerUpHandler来处理UI点击事件
  public class PhoneMapController : PhoneAppController, IPointerDownHandler, IPointerUpHandler {
    [Header("拖拽目标")]
    [SerializeField] private RectTransform map;

    [Header("父级限制区域（Local Space 限位）")]
    [SerializeField] private RectTransform parentRect;

    [Header("拖拽事件")]
    public UnityEvent OnDragged = new UnityEvent();
    public UnityEvent OnDropped = new UnityEvent();
    public UnityEvent<MapPoint> OnMovingStart = new UnityEvent<MapPoint>();
    public UnityEvent<MapPoint> OnMovingCompleted = new UnityEvent<MapPoint>();
    [Header("位置列表")]
    [SerializeField] List<MapPoint> mapPoints;
    [Header("进度条")]
    [SerializeField] private CanvasGroup movingProgressCG;
    [SerializeField] private ProgressBarExt movingProgressBar;

    //拖拽相关变量
    private Vector2 lastTouchPosition;      // 上一帧的触摸/鼠标位置
    private bool isDragging = false;        // 是否正在拖拽
    private Vector2 mapMinLimit;            // 地图可移动的最小坐标
    private Vector2 mapMaxLimit;            // 地图可移动的最大坐标
    private MapPoint currentMapPoint;

    private PhoneMapManager phoneMapManager;

    private void Awake() {
      if (map == null)
        map = GetComponent<RectTransform>();

      if (parentRect == null)
        parentRect = map.parent as RectTransform;

      // 新增：初始化地图移动边界（基于父级Rect自动计算）
      CalculateMapBoundary();
    }
    public override void Start() {
      base.Start();
      phoneMapManager = BlueberryManager.Instance.CurrentPhoneManager._PhoneMapManager;

      movingProgressBar.SetDefaultValues(phoneMapManager._minValue, phoneMapManager._maxValue);
      // 监听事件
      movingProgressBar.OnReachedEnd.AddListener(movingCompleted);
      if (mapPoints != null) {
        foreach (MapPoint mapPoint in mapPoints) {
          mapPoint.Callback += () => { StartMoving(mapPoint); };
          mapPoint.MapButton.onClick.AddListener(mapPoint.Callback.Invoke);
        }
      }
    }
    private void OnDestroy() {
      if (BlueberryManager.Instance.CurrentPhoneManager._PhoneMapManager._PhoneMapController != this) return; //I am just a shadow copy in scene, not singleton controller
      movingProgressBar.OnReachedEnd.RemoveListener(movingCompleted);
      if (mapPoints != null) {
        foreach (MapPoint mapPoint in mapPoints) {
          mapPoint.MapButton.onClick.RemoveListener(mapPoint.Callback.Invoke);
        }
      }
    }

    // 新增：核心更新逻辑 - 处理拖拽输入
    private void Update() {
      if (isDragging) {
        HandleMouseDrag();
      }
    }

    // ----- 拖拽核心逻辑 -----
    /// <summary>
    /// 计算地图可移动的边界（基于父级Rect自动适配）
    /// </summary>
    private void CalculateMapBoundary() {
      if (map == null || parentRect == null) return;

      // 计算父级可视区域的半宽/半高
      float parentHalfWidth = parentRect.rect.width / 2f;
      float parentHalfHeight = parentRect.rect.height / 2f;

      // 计算地图超出父级的部分
      float mapHalfWidth = map.rect.width / 2f;
      float mapHalfHeight = map.rect.height / 2f;

      // 地图可移动的边界：保证地图不会拖出父级可视区域
      mapMinLimit = new Vector2(parentHalfWidth - mapHalfWidth, parentHalfHeight - mapHalfHeight);
      mapMaxLimit = new Vector2(mapHalfWidth - parentHalfWidth, mapHalfHeight - parentHalfHeight);
    }

    /// <summary>
    /// 处理PC端鼠标拖拽
    /// </summary>
    private void HandleMouseDrag() {
      Vector2 currentMousePos = Input.mousePosition;
      // 计算偏移量（反向：鼠标右拖→地图左移）
      Vector2 offset = (currentMousePos - lastTouchPosition) * phoneMapManager._dragSensitivity;

      // 移动地图并触发拖拽事件
      MoveMap(offset);
      OnDragged?.Invoke();

      // 更新上一帧位置
      lastTouchPosition = currentMousePos;
    }

    /// <summary>
    /// 处理移动端触摸拖拽
    /// </summary>
    private void HandleTouchDrag() {
      Touch touch = Input.GetTouch(0);
      if (touch.phase == TouchPhase.Moved) {
        Vector2 currentTouchPos = touch.position;
        Vector2 offset = (lastTouchPosition - currentTouchPos) * phoneMapManager._dragSensitivity;

        MoveMap(offset);
        OnDragged?.Invoke();

        lastTouchPosition = currentTouchPos;
      }
    }

    /// <summary>
    /// 移动地图并限制边界
    /// </summary>
    /// <param name="offset">偏移量</param>
    private void MoveMap(Vector2 offset) {
      // 转换为UI本地空间的锚点位置
      Vector2 newPos = map.anchoredPosition;
      newPos += new Vector2(offset.x, offset.y);

      // 限制在计算好的边界内
      newPos.x = Mathf.Clamp(newPos.x, mapMinLimit.x, mapMaxLimit.x);
      newPos.y = Mathf.Clamp(newPos.y, mapMinLimit.y, mapMaxLimit.y);

      // 应用最终位置
      map.anchoredPosition = newPos;
    }

    // ----- UI事件接口实现 -----
    /// <summary>
    /// 鼠标/触摸按下时触发
    /// </summary>
    public void OnPointerDown(PointerEventData eventData) {
      isDragging = true;
      // 记录初始位置（兼容UI事件系统的位置，更精准）
      lastTouchPosition = eventData.position;
    }

    /// <summary>
    /// 鼠标/触摸抬起时触发
    /// </summary>
    public void OnPointerUp(PointerEventData eventData) {
      if (isDragging) {
        isDragging = false;
        OnDropped?.Invoke(); // 触发拖拽结束事件
      }
    }

    // 可选：如果父级Rect尺寸变化，重新计算边界
    private void OnRectTransformDimensionsChange() {
      CalculateMapBoundary();
    }

    public void SetCurrentMap(BlueberryDictionary.MAIN_SCENE targetScene) {
      MapPoint newMapPoint = getMap(targetScene);
      if (newMapPoint != null) currentMapPoint = newMapPoint;
    }
    public void StartMoving(BlueberryDictionary.MAIN_SCENE targetScene) {
      StartMoving(getMap(targetScene));
    }
    public void StartMoving(MapPoint mapPoint) {
      if (mapPoint == currentMapPoint) return;

      currentMapPoint = mapPoint;
      movingProgressCG.Enable(true);
      OnMovingStart?.Invoke(currentMapPoint);
      if (movingProgressBar == null) {
        Debug.LogError("未绑定ProgressBarExt组件！");
        return;
      }

      //重置进度条
      movingProgressBar.ResetBar(true, true);

      // 换算speed
      int calculatedSpeed = CalculateSpeedFromDuration(phoneMapManager.progressDuration);
      movingProgressBar.TargetBar.speed = calculatedSpeed;
      movingProgressBar.UpdateBarValue(phoneMapManager._maxValue, false);
      //Debug.Log("启动加载");
    }
    private void movingCompleted() {
      movingProgressCG.Disable(false);
      OnMovingCompleted.Invoke(currentMapPoint);
      BlueberryManager.Instance.CurrentPhoneManager.ClosePhone();
      GameManager.Instance._SaveLoadManager.LoadSceneSingle((int)currentMapPoint.Scene, null);
      currentMapPoint = null;
    }
    private MapPoint getMap(BlueberryDictionary.MAIN_SCENE targetScene) {
      MapPoint mapPoint = mapPoints.Where(r => r.Scene == targetScene).FirstOrDefault();
      return mapPoint;
    }

    /// <summary>
    /// 换算方法：总时长 → speed（百分比/秒），返回整数类型
    /// </summary>
    /// <param name="duration">你想要的总耗时（秒）</param>
    /// <returns>对应的speed整数值</returns>
    private int CalculateSpeedFromDuration(float duration) {
      if (duration <= 0) {
        Debug.LogWarning("时长不能小于等于0，默认设为5秒");
        duration = 5f;
      }
      float percentageRange = phoneMapManager._maxValue - phoneMapManager._minValue;
      float speedFloat = percentageRange / duration;

      int speedInt = Mathf.RoundToInt(speedFloat);

      if (speedInt <= 0) {
        speedInt = 1;
        Debug.LogWarning($"计算出的speed为{speedInt}，已强制设为1（避免动画无响应）");
      }

      return speedInt;
    }
  }
}