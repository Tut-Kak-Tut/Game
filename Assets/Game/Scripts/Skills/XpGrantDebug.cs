using Game.CoreRuntime;
using UnityEngine;

namespace Game.Skills
{
    public class XpGrantDebug : MonoBehaviour
    {
        [SerializeField] private int xpPerPress = 25;
        [SerializeField] private KeyCode grantKey = KeyCode.X;

        private void Update()
        {
            if (Input.GetKeyDown(grantKey))
            {
                GameSession.Instance?.EventBus.Publish(new XpGrantedEvent(xpPerPress, "debug_key"));
            }
        }
    }
}
