using UnityEngine;

public enum MoveType { Walk, Jump }

[System.Serializable]
public class AllyMovementStep
{
    public Transform targetPoint;
    public MoveType moveType;
}
