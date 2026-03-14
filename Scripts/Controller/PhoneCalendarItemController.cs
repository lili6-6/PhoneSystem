using Halabang.Plugin;
using Unity.VisualScripting;
using UnityEngine;

namespace Halabang.Blueberry.pp {
  public class PhoneCalendarItemController : MonoBehaviour {
    [SerializeField] private TextMeshExtend title;
    [SerializeField] private TextMeshExtend brief;
    [SerializeField] private TextMeshExtend description; //可为空

    public void InitialItem(ScheduleTask scheduleTask) {
      if (scheduleTask == null) return;

      title.SetText(scheduleTask.Content.Title);
      brief.SetText(scheduleTask.Content.Brief);
      if (description && string.IsNullOrWhiteSpace(scheduleTask.Content.Content) == false) description.SetText(scheduleTask.Content.Content);
    }
  }
}
