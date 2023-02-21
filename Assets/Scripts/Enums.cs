public class Enums
{
    public enum CombatPhase
    {
        Idle,
        UnitSelection,
        TurnStart,
        ActionSelection,
        MovementPlanning,
        Movement,
        AttackPlanning,
        Attack,
        TurnEnd
    }

    public enum Faction
    {
        Player,
        Ally,
        Enemy
    }

    public enum NavState
    {
        Walkable,
        Unwalkable
    }
}