using UnityEngine;
namespace Ltg8.Dialogue
{
    public class NpcDialogueSystem : MonoBehaviour
    {
        [SerializeField] private NpcDialogueBubbleView dialogueBubble;
        
        public void ShowText(GameObject npc, string text)
        {
            NpcDialogueBubbleView instance = npc.GetComponentInChildren<NpcDialogueBubbleView>();
            if (instance == null)
            {
                Renderer r = npc.GetComponentInChildren<Renderer>();
                instance = Instantiate(dialogueBubble, npc.transform);
                instance.transform.localPosition = new Vector3(0, r.bounds.size.y, 0); // move to top of object
            }
            instance.text.SetText(text);
        }

        public void ClearText(GameObject npc)
        {
            NpcDialogueBubbleView instance = npc.GetComponentInChildren<NpcDialogueBubbleView>();
            
            if (instance != null)
                Destroy(instance.gameObject);
        }
    }
}
