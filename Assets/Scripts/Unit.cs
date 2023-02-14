using UnityEngine;

public class Unit : MonoBehaviour
{
	#region Enumerations

	public enum Faction
	{
		Player,
		Ally,
		Enemy
	}

	#endregion

	#region Attributes

	[SerializeField] private string _name;
	[SerializeField] private Sprite _avatar;
	[SerializeField] private Faction _faction;
	[SerializeField] private int _health;
	[SerializeField] private int _healthMax;

	#endregion

	#region Properties

	public Sprite Avatar => _avatar;
	public string Name => _name;
	public int Health => _health;
	public int HealthMax => _healthMax;

	#endregion
}