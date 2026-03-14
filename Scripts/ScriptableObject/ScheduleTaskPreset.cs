using UnityEngine;
using System.Collections.Generic;
using Halabang.Story;

namespace Halabang.Blueberry.pp {
  [CreateAssetMenu(fileName = "Schedule_", menuName = "Halabang/Blueberry/Schedule Preset")]
  public class ScheduleTaskPreset : ScriptableObject {
    public int WeekNum => weekNum;
    public PhoneDictionary.DayOfWeek TargetDayOfWeek => targetDayOfWeek;
    public PhoneDictionary.ScheduleType ScheduleType => ScheduleType;
    public CopywritingPreset Content => content;

    [SerializeField] private int weekNum;
    [SerializeField] private PhoneDictionary.DayOfWeek targetDayOfWeek;
    [SerializeField] private PhoneDictionary.ScheduleType scheduleType;
    [SerializeField] private CopywritingPreset content;
  }
}