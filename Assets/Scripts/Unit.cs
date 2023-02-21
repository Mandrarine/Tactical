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

	public float jumpSpeed = 1.0f;
	public float lerpSpeed = 1.0f;
	public float translateSpeed = 1.0f;
	public float teleportDelay = 1.0f;
	public MovementType movementType;
	public int moveRange = 1;
	public float jumpHeight = 1;
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

	/*
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
	*/

	private IEnumerator MoveUnitAlongPathInternal(List<Astar.Node> path)
	{
		_moving = true;

		Astar.Node currentNode;
		Astar.Node nextNode;
		int index = 0;

		transform.position = path[index].WorldPos;

		do
		{
			currentNode = path[index];
			nextNode = path[index + 1];

			transform.LookAt(new Vector3(nextNode.WorldPos.x, currentNode.WorldPos.y, nextNode.WorldPos.z));

			float verticalDifference = Mathf.Abs(currentNode.WorldPos.y - nextNode.WorldPos.y);

			//if (nextNode.WorldPos.y == currentNode.WorldPos.y)
			if (verticalDifference == 0)
			{
				switch (movementType)
				{
					case MovementType.Translate:
						yield return StartCoroutine(TranslateToPosition(nextNode.WorldPos));
						break;
					case MovementType.Lerp:
						yield return StartCoroutine(LerpToPosition(nextNode.WorldPos));
						break;
					case MovementType.Teleport:
						yield return StartCoroutine(TeleportToPosition(nextNode.WorldPos));
						break;
					default:
						yield return StartCoroutine(TranslateToPosition(nextNode.WorldPos));
						break;
				}
			}
			else
				yield return StartCoroutine(JumpToPosition(currentNode.WorldPos, nextNode.WorldPos));

			index++;
		}
		while (index < path.Count - 1);

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
		_animator.SetFloat("MoveSpeed", 1);

		while (transform.position != targetPosition)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * translateSpeed);
			yield return null;
		}

		_animator.SetFloat("MoveSpeed", 0);
	}

	private IEnumerator JumpToPosition(Vector3 originPosition, Vector3 targetPosition)
	{
		float t = 0.0f;

		//_animator.SetBool("Jumping", true);

		while (t != 1.0f)
		{
			t = Mathf.Clamp01(t += Time.deltaTime * jumpSpeed);
			//_animator.SetFloat("JumpTime", t);
			transform.position = CubicBezier(originPosition, originPosition, targetPosition + Vector3.up, targetPosition, t);
			yield return null;
		}

		//_animator.SetBool("Jumping", false);
	}

	private Vector3 CubicBezier(Vector3 p0, Vector3 c1, Vector3 c2, Vector3 p3, float t)
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float ttt = tt * t;
		float uuu = uu * u;

		Vector3 p = uuu * p0;
		p += 3 * uu * t * c1;
		p += 3 * u * tt * c2;
		p += ttt * p3;

		return p;
	}

	#endregion
}