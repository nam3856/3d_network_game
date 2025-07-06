using TMPro;
using UnityEngine;
public class QuickMatchUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _SearchText;
    [SerializeField] private GameObject SearchingIndicator;

    private void Start()
    {
        QuickMatchManager.QuickMatchCallback += HandleQuickMatchCallback;
        QuickMatchManager.OnLeaveLobby += OnLeftLobby;
    }

    private void OnDestroy()
    {

        QuickMatchManager.OnLeaveLobby -= OnLeftLobby;
        QuickMatchManager.QuickMatchCallback -= HandleQuickMatchCallback;
    }

    private void OnLeftLobby()
    {
        SearchingIndicator.SetActive(true);
    }

    private void HandleQuickMatchCallback(string text)
    {
        if (SearchingIndicator.activeSelf)
        {
            SearchingIndicator.SetActive(false);
        }

        _SearchText.text = text;
    }
}
