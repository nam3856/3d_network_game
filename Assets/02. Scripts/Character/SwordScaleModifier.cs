using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SwordScaleModifier : MonoBehaviour
{
    private float _scaleFactor = 1f;
    private float _score = 0f;
    private void OnEnable()
    {
        ScoreManager.OnDataChanged += HandleScoreChanged;
    }
    private void OnDisable()
    {
        ScoreManager.OnDataChanged -= HandleScoreChanged;
    }

    private void HandleScoreChanged()
    {
        _score = ScoreManager.Instance.Score;
        _scaleFactor = 1f + _score / 100000;
        ChangeScale();
    }
    private void ChangeScale()
    {
        transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
    }
}