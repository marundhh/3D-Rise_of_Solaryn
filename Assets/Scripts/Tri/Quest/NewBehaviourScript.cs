using UnityEngine;

public class NPC : MonoBehaviour
{
    public string npcName = "NPC_A";

    public void Interact()
    {
        // Gửi sự kiện khi người chơi nói chuyện
        GameEventSystem.Dispatch(new TalkToNPCEvent(npcName));
    }

    private void OnMouseDown() // click chuột vào NPC
    {
        Interact();
    }
}
