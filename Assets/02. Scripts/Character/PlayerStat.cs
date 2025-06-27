using System;

[Serializable]
public class PlayerStat
{
    public float MoveSpeed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float WalkSpeed => MoveSpeed;
    public float SprintSpeed => MoveSpeed * 1.5f;
    public float AttackRange = 2f;
    public int AttackDamage = 10;
    public float AttackCooldown = 0.6f;

    public float MouseRotationSpeed = 10f;
    public float GamepadRotationSpeed = 500f;
}