using System.Text;
using UnityEngine;

namespace Game.CoreRuntime
{
    public class CoreRuntimeDiagnostics : MonoBehaviour
    {
        [SerializeField] private bool showOverlay = true;
        [SerializeField] private Rect overlayRect = new Rect(12f, 12f, 480f, 250f);

        private void OnGUI()
        {
            if (!showOverlay || GameSession.Instance == null)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SaveVersion: {SaveService.CurrentVersion}");
            sb.AppendLine("Recent Events:");

            var events = GameSession.Instance.EventBus.GetRecentEvents();
            int start = Mathf.Max(0, events.Count - 10);
            for (int i = start; i < events.Count; i++)
            {
                sb.AppendLine($"- {events[i].GetType().Name}");
            }

            GUI.Box(overlayRect, sb.ToString());
        }
    }
}
