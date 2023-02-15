using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : MonoBehaviour
{
	#region Enumerations

	public enum Faction
	{
		Player,
		Ally,
		Enemy
	}

	public enum MovementType
	{
		Translate,
		Lerp,
		Teleport
	}

	#endregion

	#region Fields

	public float lerpSpeed = 1.0f;
	public float translateSpeed = 1.0f;
	public float teleportDelay = 1.0f;
	public MovementType movementType;
	public Color color;

	[SerializeField] private string _name;
	[SerializeField] private int _level;
	[SerializeField] private Sprite _avatar;
	[SerializeField] private Faction _faction;
	[SerializeField] private int _health;
	[SerializeField] private int _healthMax;
	[SerializeField] private int _mana;
	[SerializeField] private int _manaMax;

	private Animator _animator;
	private bool _moving;

	#endregion

	#region Properties

	public Sprite Avatar => _avatar;
	public string Name => _name;
	public int Level => _level;
	public int Health => _health;
	public int HealthMax => _healthMax;
	public int Mana => _mana;
	public int ManaMax => _manaMax;

	#endregion

	#region Unity

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
	}

	#endregion

	#region Methods

	public void MoveUnitAlongPath(List<Astar.Node> path)
	{
		if (_moving)
			return;

		StartCoroutine(MoveUnitAlongPathInternal(path));
	}

	private IEnumerator MoveUnitAlongPathInternal(List<Astar.Node> path)
	{
		_moving = true;
		_animator.SetBool("Move", true);

		Astar.Node node = path.First();
		transform.position = node.WorldPos;
		path.Remove(node);

		while (path.Count > 0)
		{
			node = path.First();
			transform.LookAt(node.WorldPos);

			switch (movementType)
			{
				case MovementType.Translate:
					yield return StartCoroutine(TranslateToPosition(node.WorldPos));
					break;
				case MovementType.Lerp:
					yield return StartCoroutine(LerpToPosition(node.WorldPos));
					break;
				case MovementType.Teleport:
					yield return StartCoroutine(TeleportToPosition(node.WorldPos));
					break;
				default:
					yield return StartCoroutine(TranslateToPosition(node.WorldPos));
					break;
			}

			path.Remove(node);
		}

		_moving = false;
		_animator.SetBool("Move", false);
	}

	private IEnumerator TeleportToPosition(Vector3 targetPosition)
	{
		transform.position = targetPosition;
		yield return new WaitForSeconds(teleportDelay);
	}

	private IEnumerator LerpToPosition(Vector3 targetPosition)
	{
		while (transform.position != targetPosition)
		{
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
			yield return null;
		}
	}

	private IEnumerator TranslateToPosition(Vector3 targetPosition)
	{
		while (transform.position != targetPosition)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * translateSpeed);
			yield return null;
		}
	}

	#endregion
}