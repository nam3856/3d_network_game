using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HUDManager : MonoBehaviour
{
    [SerializeField] private Image _hpBarImage;
    [SerializeField] private Image _staminaBarImage;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _staminaText;
    [SerializeField] private TextMeshProUGUI _nickNameText;
    public static HUDManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        InitializeItems();
    }

    private void InitializeItems()
    {
        _hpBarImage.fillAmount = 1;
        _staminaBarImage.fillAmount = 1;
        _hpText.text = "100/100";
        _staminaText.text = "100/100";
    }

    public void UpdateHpUI(float current, float max)
    {
        _hpBarImage.fillAmount = current / max;
        _hpText.text = $"{current.ToString("F2")}/{max.ToString("F2")}";
    }

    public void UpdateStaminaUI(float current, float max)
    {
        _staminaBarImage.fillAmount=current / max;
        _staminaText.text = $"{current.ToString("F2")}/{max.ToString("F2")}";
    }

    public void SetNickname(string nickName)
    {
        _nickNameText.text = nickName;
    }
}
