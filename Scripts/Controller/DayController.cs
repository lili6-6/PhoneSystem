using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Halabang.Plugin;
using System.Collections.Generic;

namespace Halabang.Blueberry.pp {
  /// <summary>
  /// 日Controller控制单日的状态刷新
  /// </summary>
  public enum DayState {
    Past,
    Future,
    Selected,
    Schedule
  }

  // DayController：控制每一天的状态
  public class DayController : MonoBehaviour {
    [Header("事件")]
    [SerializeField] public UnityEvent PastEvent;
    [SerializeField] public UnityEvent FutureEvent;
    [SerializeField] public UnityEvent BeselectedEvent;
    [SerializeField] public UnityEvent ScheduleEvent;

    [Header("Sprite")]
    [SerializeField] private Sprite pastSprite;
    [SerializeField] private Sprite futureSprite;
    [SerializeField] private Sprite beSelectedSprite;
    [SerializeField] private Sprite scheduleSprite;

    [Header("文本")]
    [SerializeField] private TextMeshExtend text;

    public PhoneDictionary.DayOfWeek DayOfWeek { get; private set; }
    public void ChangeDayOfWeek(PhoneDictionary.DayOfWeek dayOfWeek) {
      DayOfWeek = dayOfWeek;
    }
    private PhoneCalendarController phoneCalendarController;
    private PhoneCalendarManager phoneCalendarManager;

    void Start() {
      StartCoroutine(InitController());
    }

    private IEnumerator InitController() {
      yield return new WaitForSeconds(0.15f);
      phoneCalendarManager = BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager;

      phoneCalendarController = BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.PhoneCalendarController;

      phoneCalendarController._dayControllerList.Add(this);

      //IndexToDay();
      int indexInWeek = GetIndex();
      ChangeDayOfWeek((PhoneDictionary.DayOfWeek)indexInWeek);

      if (indexInWeek == 0) indexInWeek = 7; // Sunday 显示为 7
      text.SetText(indexInWeek.ToString());

      // 初始化状态
      RefreshState(phoneCalendarManager.CurrentDay);
    }

    // 更新文本显示
    //public void UpdateText()
    //{
    //    int tex = GetIndex();
    //    if (tex == 0) tex = 7; // Sunday 显示为 7
    //    text.SetText(tex.ToString());
    //}

    // 根据索引设置星期几
    //public void IndexToDay()
    //{
    //    switch (GetIndex())
    //    {
    //        case 0: changeDayOfWeek("Sunday"); break;
    //        case 1: changeDayOfWeek("Monday"); break;
    //        case 2: changeDayOfWeek("Tuesday"); break;
    //        case 3: changeDayOfWeek("Wednesday"); break;
    //        case 4: changeDayOfWeek("Thursday"); break;
    //        case 5: changeDayOfWeek("Friday"); break;
    //        case 6: changeDayOfWeek("Saturday"); break;
    //        default: changeDayOfWeek("Unknown"); break;
    //    }
    //}

    // 获取当前对象在父对象中的索引
    public int GetIndex() {
      Transform parent = transform.parent;
      for (int i = 0; i < parent.childCount; i++)
        if (parent.GetChild(i).gameObject == gameObject) return i;
      return -1;
    }

    // 将星期几转换为索引
    //public int DayToIndex(string day)
    //{
    //    switch (day)
    //    {
    //        case "Sunday": return 0;
    //        case "Monday": return 1;
    //        case "Tuesday": return 2;
    //        case "Wednesday": return 3;
    //        case "Thursday": return 4;
    //        case "Friday": return 5;
    //        case "Saturday": return 6;
    //        default: return -1;
    //    }
    //}

    // 点击某一天
    public void OnClickDay() {
      phoneCalendarController.SelectDay(new KeyValuePair<int, PhoneDictionary.DayOfWeek>(phoneCalendarController.displayWeekNum, DayOfWeek), true);
    }

    // 设置状态
    public void SetState(DayState state, ScheduleTask schedule = null) {
      switch (state) {
        case DayState.Past:
          PastEvent?.Invoke();
          GetComponent<ButtonManagerExt>().SetBackground(pastSprite);
          break;
        case DayState.Future:
          FutureEvent?.Invoke();
          GetComponent<ButtonManagerExt>().SetBackground(futureSprite);
          break;
        case DayState.Selected:
          BeselectedEvent?.Invoke();
          GetComponent<ButtonManagerExt>().SetBackground(beSelectedSprite);
          break;
        case DayState.Schedule:
          ScheduleEvent?.Invoke();
          GetComponent<ButtonManagerExt>().SetBackground(scheduleSprite);
          //if (schedule != null)
          //    phoneCalendarController.RefreshSchedule(schedule);
          break;
      }
    }

    // 刷新状态
    public void RefreshState(KeyValuePair<int, PhoneDictionary.DayOfWeek> selectedDay) {
      // 1. 判断是否选中
      if (DayOfWeek == selectedDay.Value) {
        //Debug.Log("displayWeekNum:" + phoneCalendarController.displayWeekNum + "currentWeekNum:" + phoneCalendarManager.currentWeekNum);
        SetState(DayState.Selected);
        return; // Selected 优先
      }

      // 2. 判断日程（未选中才显示日程）
      //Debug.Log(phoneCalendarManager);
      ScheduleTask schedule = phoneCalendarManager.GetScheduleForDay(
        new KeyValuePair<int, PhoneDictionary.DayOfWeek>(phoneCalendarController.displayWeekNum, DayOfWeek)          
      );
      if (schedule != null) {
        SetState(DayState.Schedule, schedule);
        return;
      }

      // 3. 判断 Past / Future
      int currentDayIndex = (int)phoneCalendarManager.CurrentDay.Value;
      int displayWeek = phoneCalendarController.displayWeekNum;
      int currentWeek = phoneCalendarManager.CurrentDay.Key;

      if (displayWeek < currentWeek || (displayWeek == currentWeek && (int)DayOfWeek < currentDayIndex))
        SetState(DayState.Past);
      else
        SetState(DayState.Future);
    }

  }
}
