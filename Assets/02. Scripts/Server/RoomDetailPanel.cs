using UnityEngine;
using TMPro;

public class RoomDetailPanel : MonoBehaviour
{
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playerCountText;
    public TextMeshProUGUI hostNameText;

    public void SetRoomDetail(string name, int current, int max, string host)
    {
        roomNameText.text = $"{name}";
        playerCountText.text = $"{current} / {max}";
        hostNameText.text = $"방장: {host}";
    }
}