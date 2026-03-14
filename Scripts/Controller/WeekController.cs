using System.Collections;
using UnityEngine;
using Halabang.Plugin;
using System.Collections.Generic;

namespace Halabang.Blueberry.pp {
  /// <summary>
  /// 周Controller控制周的预览刷新
  /// </summary>
  public class WeekController : MonoBehaviour {
    public PhoneCalendarController PhoneCalendarController { get; set; }

    [SerializeField] private TextMeshExtend weekNumText;

    private KeyValuePair<int, PhoneDictionary.DayOfWeek> targetDay;

    public void UpdateWeekNum() {
      weekNumText.SetText("Week " + PhoneCalendarController.displayWeekNum);
    }
    public void PreviousWeek() {
      if (PhoneCalendarController.displayWeekNum <= 1) return;
      //selectedDay = null;
      PhoneCalendarController.changeDisplayWeekNum(PhoneCalendarController.displayWeekNum - 1);
      if (PhoneCalendarController.displayWeekNum != BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay.Key) {
        targetDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(PhoneCalendarController.displayWeekNum, PhoneDictionary.DayOfWeek.Sunday);
      } else {
        targetDay = BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay;
      }
      UpdateWeekNum();
      PhoneCalendarController.ClearList();
      PhoneCalendarController.RefreshAllDays(targetDay);

    }
    public void NextWeek() {
      if (PhoneCalendarController.displayWeekNum >= BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.MaxWeekNum) {
        return;
      }
      //selectedDay = null;
      PhoneCalendarController.changeDisplayWeekNum(PhoneCalendarController.displayWeekNum + 1);
      if (PhoneCalendarController.displayWeekNum != BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay.Key) {
        targetDay = new KeyValuePair<int, PhoneDictionary.DayOfWeek>(PhoneCalendarController.displayWeekNum, PhoneDictionary.DayOfWeek.Sunday);
      } else {
        targetDay = BlueberryManager.Instance.CurrentPhoneManager._PhoneCalendarManager.CurrentDay;
      }
      UpdateWeekNum();
      PhoneCalendarController.ClearList();
      PhoneCalendarController.RefreshAllDays(targetDay);

    }
  }
}