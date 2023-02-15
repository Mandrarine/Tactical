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

	[SerializeField] private string _name;
	[SerializeField] private Sprite _avatar;
	[SerializeField] private Faction _faction;
	[SerializeField] private int _health;
	[SerializeField] private int _healthMax;

	private bool _moving;
	public Vector3 startPos;

	#endregion

	#region Properties

	public Sprite Avatar => _avatar;
	public string Name => _name;
	public int Health => _health;
	public int HealthMax => _healthMax;

	#endregion

	private void Start()
	{
		startPos = transform.position;
	}

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

		transform.position = startPos;

		while (path.Count > 0)
		{
			Astar.Node targetNode = path.First();

			transform.LookAt(targetNode.WorldPos);

			switch (movementType)
			{
				case MovementType.Translate:
					yield return StartCoroutine(TranslateToPosition(targetNode.WorldPos));
					break;
				case MovementType.Lerp:
					yield return StartCoroutine(LerpToPosition(targetNode.WorldPos));
					break;
				case MovementType.Teleport:
					yield return StartCoroutine(TeleportToPosition(targetNode.WorldPos));
					break;
				default:
					yield return StartCoroutine(TranslateToPosition(targetNode.WorldPos));
					break;
			}

			path.Remove(targetNode);
		}

		_moving = false;
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