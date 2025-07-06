using TMPro;
using UnityEngine;

public class UI_ScoreSlot : MonoBehaviour
{
    public TextMeshProUGUI RankTextUI;
    public TextMeshProUGUI NicknameTextUI;
    public TextMeshProUGUI ScoreTextUI;
    public TextMeshProUGUI KillTextUI;

    public void Set(string rank, string nickname, string score, string kill)
    {
        RankTextUI.text = rank;
        NicknameTextUI.text = nickname;
        ScoreTextUI.text = score;
        KillTextUI.text = kill;
    }
}