using TMPro;
using UnityEngine;
public class QuickMatchUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _SearchText;
    [SerializeField] private GameObject SearchingIndicator;

    private void Start()
    {
        QuickMatchManager.QuickMatchCallback += HandleQuickMatchCallback;
    }

    private void OnDestroy()
    {
        QuickMatchManager.QuickMatchCallback -= HandleQuickMatchCallback;
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
