using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerShakingAbility : PlayerAbility
{
    public Transform Target;
    public float Strength;
    public float Duration;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    public void Shake()
    {
        _impulseSource.GenerateImpulse(Vector3.back);
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPosition = Target.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < Duration)
        {
            Vector3 randomPosition = Random.insideUnitSphere.normalized * Strength;
            randomPosition.y = originalPosition.y;
            Target.localPosition = randomPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Target.localPosition = originalPosition; // Reset position after shaking
    }
}