using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    public Button Select_Male;
    public Button Select_Female;
    public GameObject Select_FemalePrefab;
    public GameObject Select_MalePrefab;

    private CharacterType _currentSelected;

    private void Awake()
    {
        Select_Male.onClick.AddListener(() => SelectCharacter(CharacterType.M));
        Select_Female.onClick.AddListener(() => SelectCharacter(CharacterType.F));

        string saved = PlayerPrefs.GetString("SelectedCharacterType", CharacterType.M.ToString());
        _currentSelected = (CharacterType)System.Enum.Parse(typeof(CharacterType), saved);
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void OnDisable()
    {
        Select_FemalePrefab.SetActive(false);
        Select_MalePrefab.SetActive(false);
    }
    private void SelectCharacter(CharacterType selected)
    {
        _currentSelected = selected;
        PlayerPrefs.SetString("SelectedCharacterType", selected.ToString());

        UpdateUI();
    }

    private void UpdateUI()
    {
        Select_Male.interactable = _currentSelected != CharacterType.M;
        Select_Female.interactable = _currentSelected != CharacterType.F;


        Select_FemalePrefab.SetActive(_currentSelected == CharacterType.F);
        Select_MalePrefab.SetActive(_currentSelected == CharacterType.M);

        if (Select_FemalePrefab.activeSelf)
        {
            Select_FemalePrefab.GetComponent<Animator>().SetBool("IsGrounded", true);
            Select_FemalePrefab.GetComponent<Animator>().SetTrigger($"Attack{Random.Range(1, 4)}");
        }
        if (Select_MalePrefab.activeSelf)
        {
            Select_MalePrefab.GetComponent<Animator>().SetBool("IsGrounded", true);
            Select_MalePrefab.GetComponent<Animator>().SetTrigger($"Attack{Random.Range(1, 4)}");
        }
    }
}
