using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float WalkSpeed => MoveSpeed;
    public float SprintSpeed => MoveSpeed * 1.5f;
    public float MoveMultipler = 1.5f;
    public float MoveBuffTime = 5f;

    [Header("Combat")]
    public float AttackRange = 2f;
    public float AttackMinDamage = 10;
    public float AttackMaxDamage = 15;
    public float AttackCooldown = 0.6f;
    public float AttackBuffTime = 5f;
    public float AttackBuffStat = 1.5f;

    [Header("Stemina")]
    public float MaxStamina = 100f;
    public float RunStaminaPerSecond = 10f;
    public float AttackStamina = 20f;
    public float JumpStamina = 10f;
    public float StaminaRecoverPerSecond = 20f;
    public float StaminaDelay = 0.5f;

    [Header("Health")]
    public float MaxHealth = 100f;
    public float HealthRecoverPerSecond = 3f;
    public float RespawnTime = 5f;


    [Header("Camera")]
    public float MouseRotationSpeed = 10f;
    public float GamepadRotationSpeed = 500f;
}