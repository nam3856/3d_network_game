using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    public List<UI_ScoreSlot> Slots;
    public UI_ScoreSlot MySlot;

    private void Start()
    {
        ScoreManager.OnDataChanged += Refresh;
    }

    private void Refresh()
    {
        Dictionary<string, (int, int)> scores = ScoreManager.Instance.Scores;
        var sortedScores = scores.ToList().OrderByDescending(x => x.Value.Item1).ToList();

        for (int i = 0; i < Slots.Count; i++)
        {
            if (i < sortedScores.Count)
            {
                Slots[i].Set($"{i + 1}", sortedScores[i].Key, sortedScores[i].Value.Item1.ToString("N0"), sortedScores[i].Value.Item2.ToString("N0"));
            }
            else
            {
                Slots[i].Set("", "", "", "");
            }
        }

        // 내 점수 등록
        string myKey = $"{Photon.Pun.PhotonNetwork.NickName}_{Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber}";
        int myIndex = sortedScores.FindIndex(x => x.Key == myKey);
        if (myIndex != -1)
        {
            var myData = sortedScores[myIndex];
            MySlot.Set($"{myIndex + 1}", myData.Key, myData.Value.Item1.ToString("N0"), myData.Value.Item2.ToString("N0"));
        }
    }
}
