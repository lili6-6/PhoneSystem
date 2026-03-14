
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Halabang.Blueberry.pp {
  /// <summary>
  /// 日历appManager控制日期进度
  /// </summary>
  public class PhoneCalendarManager : MonoBehaviour {
    public List<ScheduleTask> Schedules => getScheduleTasks();
    public PhoneCalendarController PhoneCalendarController => phoneCalendarController;
    public KeyValuePair<int, PhoneDictionary.DayOfWeek> CurrentDay { get; private set; }
    public int MaxWeekNum => maxWeekNum;

    [Header("设定")]
    [SerializeField] PhoneCalendarController phoneCalendarController;
    [Header("日程列表")]
    [SerializeField] private List<ScheduleTaskPreset> staticSchedules = new List<ScheduleTaskPreset>();
    [SerializeField] private int defaultWeekNum = 1;
    [SerializeField] private int maxWeekNum = 20;

    private List<ScheduleTask> dynamicSchedules; //动态获取的日历，包括从存档中获取，以及从runtime中实时添加

    void Start() {
      //StartCoroutine(InitManager());
      ResetCalendar();
    }
    //public void RegisterController(PhoneCalendarController controller) {
    //  phoneCalendarController = controller;
    //}

    /// <summary>
    /// 经过一天，自动切换到下一天，并刷新日历显示
    /// </summary>
    public void GoToNextDay() {
      switch (CurrentDay.Value) {
        case PhoneDictionary.DayOfWeek.Sunday: 
          CurrentDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek> (CurrentDay.Key, PhoneDictionary.DayOfWeek.Monday); 
          break;
        case PhoneDictionary.DayOfWeek.Monday: 
          CurrentDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(CurrentDay.Key, PhoneDictionary.DayOfWeek.Tuesday); 
          break;
        case PhoneDictionary.DayOfWeek.Tuesday: 
          CurrentDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(CurrentDay.Key, PhoneDictionary.DayOfWeek.Wednesday); 
          break;
        case PhoneDictionary.DayOfWeek.Wednesday: 
          CurrentDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(CurrentDay.Key, PhoneDictionary.DayOfWeek.Thursday); 
          break;
        case PhoneDictionary.DayOfWeek.Thursday: 
          CurrentDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(CurrentDay.Key, PhoneDictionary.DayOfWeek.Friday); 
          break;
        case PhoneDictionary.DayOfWeek.Friday: 
          CurrentDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(CurrentDay.Key, PhoneDictionary.DayOfWeek.Saturday); 
          break;
        case PhoneDictionary.DayOfWeek.Saturday:
          GoToNextWeek();
          return;
      }

      // 刷新日历状态
      //phoneCalendarController?.RefreshAllDays(currentDayOfWeek.ToString());
      //phoneCalendarController?.RefreshSchedule(GetScheduleForDay(currentDayOfWeek, currentWeekNum));
      phoneCalendarController.SelectDay(CurrentDay, false);
    }

    /// <summary>
    /// 经过一周，周数+1，切换到周日
    /// </summary>
    public void GoToNextWeek() {
      CurrentDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(CurrentDay.Key + 1, PhoneDictionary.DayOfWeek.Sunday);

      if (phoneCalendarController != null) {
        phoneCalendarController.changeDisplayWeekNum(CurrentDay.Key);
        phoneCalendarController.WeekController.UpdateWeekNum();
        //phoneCalendarController.RefreshAllDays(currentDayOfWeek.ToString());
        //phoneCalendarController.RefreshSchedule(GetScheduleForDay(currentDayOfWeek, currentWeekNum));
        phoneCalendarController.SelectDay(CurrentDay, false);
      }
    }

    /// <summary>
    /// 重置日历到第一周、周一
    /// </summary>
    public void ResetCalendar() {
      CurrentDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(defaultWeekNum, PhoneDictionary.DayOfWeek.Sunday);

      if (phoneCalendarController != null) {
        phoneCalendarController.changeDisplayWeekNum(CurrentDay.Key);
        phoneCalendarController.WeekController.UpdateWeekNum();
        phoneCalendarController.RefreshAllDays(CurrentDay);
        phoneCalendarController.RefreshSchedule(GetScheduleForDay(CurrentDay));
      }
    }

    /// <summary>
    /// 获取某天的日程，如果没有返回 null
    /// </summary>
    public ScheduleTask GetScheduleForDay(KeyValuePair<int, PhoneDictionary.DayOfWeek> targetDate) {
      return Schedules.Where(r => r.WeekNum == targetDate.Key && r.TargetDayOfWeek == targetDate.Value).FirstOrDefault();
    }
    public int meetTime(int EncounterWeek, int EncounterDay) {
      int currentTime = (CurrentDay.Key - 1) * 7 + DayToIndex(CurrentDay.Key.ToString()) + 1;
      int EncounterTime = (EncounterWeek - 1) * 7 + DayToIndex(EncounterDay.ToString()) + 1;

      return EncounterTime - currentTime <= 0 ? 0 : EncounterTime - currentTime;

    }
    public int DayToIndex(string day) {
      switch (day) {
        case "Sunday": return 0;
        case "Monday": return 1;
        case "Tuesday": return 2;
        case "Wednesday": return 3;
        case "Thursday": return 4;
        case "Friday": return 5;
        case "Saturday": return 6;
        default: return -1;
      }
    }

    //获取打开的controller面板
    public GameObject GetPanelStatus() {
      if (phoneCalendarController._panelStatus == true) {
        return phoneCalendarController.gameObject;
      }

      return null;
    }
    public void Close() {
      if (GetPanelStatus() != null)
        phoneCalendarController.CloseApp();
    }
    public void Open() {
      phoneCalendarController.OpenApp();
    }

    public void InsertSchedule(ScheduleTask newSchedule) {
      if (dynamicSchedules == null) dynamicSchedules = new List<ScheduleTask>();
      if (dynamicSchedules.Any(r => r.WeekNum == newSchedule.WeekNum && r.TargetDayOfWeek == newSchedule.TargetDayOfWeek)) {
        Debug.LogError("Cannot insert a new schedule, current day has already occupied: " + newSchedule.WeekNum + " / " + newSchedule.TargetDayOfWeek);
        return;
      }
      dynamicSchedules.Add(newSchedule);
    }
    private List<ScheduleTask> getScheduleTasks() {
      if (staticSchedules != null) {
        if (staticSchedules.GroupBy(r => new { r.WeekNum, r.TargetDayOfWeek }).Any(g => g.Count() > 1)) {
          Debug.LogError("Unexpected error, static schedule contains multiple schedule task within same day");
        }
      }

      if (dynamicSchedules == null) dynamicSchedules = new List<ScheduleTask>();

      return dynamicSchedules;
    }
  }
}
