using Halabang.Plugin;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Halabang.Utilities;

namespace Halabang.Blueberry.pp {
  /// <summary>
  /// 日历appController控制整个日历
  /// </summary>

  public class PhoneCalendarController : PhoneAppController {

    [SerializeField] private WeekController weekController;
    [SerializeField] private RectTransform dayOfWeekHolder;
    [SerializeField] private Image scheduleBg;
    [SerializeField] private RectTransform scheduleItemHolder;
    [SerializeField] private PhoneCalendarItemController scheduledPrefab;

    [Header("日程事件")]
    [SerializeField] public UnityEvent noScheduleEvent;
    [SerializeField] public UnityEvent hasScheduleEvent;

    public int displayWeekNum { get; private set; }
    public void changeDisplayWeekNum(int WeekNum) {
      displayWeekNum = WeekNum;
    }

    public WeekController WeekController => weekController;

    private List<DayController> DayControllerList = new List<DayController>();
    public List<DayController> _dayControllerList => DayControllerList;
    private KeyValuePair<int, PhoneDictionary.DayOfWeek> selectedDay;
    private List<PhoneCalendarItemController> scheduleItems = new List<PhoneCalendarItemController>();

    public void Awake() {
      weekController.PhoneCalendarController = this;
    }
    public override void Start() {
      base.Start();

      changeDisplayWeekNum(BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay.Key);

      ClearList();

      RefreshAllDays(BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay);

      weekController.UpdateWeekNum();
    }

    // 清空日程列表
    public void ClearList() {
      scheduleItemHolder.transform.DestroyChildren(false);
    }

    // 刷新日程列表
    public void RefreshSchedule(ScheduleTask schedule = null) {
      ClearList(); // 先清空
      if (schedule == null) {
        noScheduleEvent?.Invoke();
        //Debug.Log("No schedule for the selected day.");
        return;
      }

      hasScheduleEvent?.Invoke();
      //Debug.Log("Schedule found for the selected day.");
      PhoneCalendarItemController newScheduld = Instantiate(scheduledPrefab, scheduleItemHolder);
      newScheduld.InitialItem(schedule);
    }

    // 刷新所有天的状态
    public void RefreshAllDays(KeyValuePair<int, PhoneDictionary.DayOfWeek> selected) {
      foreach (var dayCtrl in _dayControllerList) {
        dayCtrl.RefreshState(selected);
      }
    }

    // 点击某天
    public void SelectDay(KeyValuePair<int, PhoneDictionary.DayOfWeek> day, bool isClick) {
      selectedDay = day;
      if (displayWeekNum != BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay.Key && !isClick) {
        return;
      }


      // 1️⃣ 只刷新 DayController 的状态（背景）
      foreach (var dayCtrl in _dayControllerList) {
        dayCtrl.RefreshState(selectedDay);
      }

      // 2️⃣ 只在“选中的这一天有日程”时刷新日程列表
      ScheduleTask schedule = GetScheduleForSelectedDay();

      RefreshSchedule(schedule); // null 会清空
    }

    // 获取选中天的日程
    private ScheduleTask GetScheduleForSelectedDay() {
      if (BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.Schedules == null) return null;

      return BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.Schedules.Where(r => r.WeekNum == selectedDay.Key && r.TargetDayOfWeek == selectedDay.Value).FirstOrDefault();
    }

    // 重置日历到当前周和当前天
    public void ResetCalendar() {
      changeDisplayWeekNum(BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay.Key);
      WeekController.UpdateWeekNum();
      selectedDay = BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay;
      RefreshAllDays(selectedDay);
      ClearList();
    }
  }
}