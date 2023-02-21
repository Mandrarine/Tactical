using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
	#region Fields

	public static CombatManager Instance;

	[SerializeField] private Enums.CombatPhase _combatPhase = Enums.CombatPhase.Idle;
	[SerializeField] private int _turnCount = 0;

	private Tile _tileActiveUnit = null;
	private Tile _tileUnitPathStart;
	private Tile _tileUnitPathEnd;
	private List<Astar.Node> _unitPath;

	[SerializeField] private Transform _moveRangeMarkersParent;
	[SerializeField] private GameObject _prefabMoveRangeMarker;
	private List<Astar.Node> _nodesInMoveRange;

	#endregion

	#region Properties

	public Enums.CombatPhase CombatPhase => _combatPhase;
	public int TurnCount => _turnCount;

	#endregion

	#region Unity

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		SetPhase(Enums.CombatPhase.TurnStart);
		StartCoroutine(CombatPhasesHandler());
	}

	private IEnumerator CombatPhasesHandler()
	{
		while (_combatPhase != Enums.CombatPhase.Idle)
		{
			if (_combatPhase == Enums.CombatPhase.UnitSelection)
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					Tile tile = GridManager.Instance.FocusedTile;
					if ((tile.Unit != null) && (tile.Unit.Faction == Enums.Faction.Player))
					{
						_tileActiveUnit = tile;
						SetPhase(Enums.CombatPhase.ActionSelection);
					}
				}
			}
			if (_combatPhase == Enums.CombatPhase.ActionSelection)
			{
				if (Input.GetKeyDown(KeyCode.A))
				{
					// Attack
					//SetPhase(Enums.CombatPhase.AttackPlanning);
				}
				else if (Input.GetKeyDown(KeyCode.M))
				{
					// Move
					_nodesInMoveRange = Astar.FindNodesInRange(_tileActiveUnit.Node, _tileActiveUnit.Unit.moveRange, _tileActiveUnit.Unit.jumpHeight);
					_nodesInMoveRange.Remove(_tileActiveUnit.Node);

					SetPhase(Enums.CombatPhase.MovementPlanning);
				}
			}
			else if (_combatPhase == Enums.CombatPhase.MovementPlanning)
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					_tileUnitPathStart = _tileActiveUnit;
					_tileUnitPathEnd = GridManager.Instance.FocusedTile;

					if ((_nodesInMoveRange != null) && _nodesInMoveRange.Contains(_tileUnitPathEnd.Node))
					{
						_unitPath = Astar.FindPath(_tileUnitPathStart.Node, _tileUnitPathEnd.Node, _tileUnitPathStart.Unit.jumpHeight);

						if (_unitPath != null)
							SetPhase(Enums.CombatPhase.Movement);
					}
				}
			}
			else if (_combatPhase == Enums.CombatPhase.Movement)
			{
				yield return StartCoroutine(_tileActiveUnit.Unit.MoveUnitAlongPath(_unitPath));

				_tileActiveUnit = _tileUnitPathEnd;
				_tileActiveUnit.Unit = _tileUnitPathStart.Unit;
				_tileUnitPathStart.Unit = null;

				SetPhase(Enums.CombatPhase.ActionSelection);
			}

			yield return null;
		}
	}

	#endregion

	#region Methods

	private void SetPhase(Enums.CombatPhase phase)
	{
		OnPhaseExit(_combatPhase);
		_combatPhase = phase;
		OnPhaseEnter(_combatPhase);

		UIManager.Instance.UpdateCombatInfo();
	}

	private void OnPhaseEnter(Enums.CombatPhase phase)
	{
		if (phase == Enums.CombatPhase.TurnStart)
		{
			_turnCount++;
			SetPhase(Enums.CombatPhase.UnitSelection);
		}
		else if (phase == Enums.CombatPhase.ActionSelection)
		{
			GridManager.Instance.SetTileSelectorLocked(true);
		}
		else if (phase == Enums.CombatPhase.MovementPlanning)
		{
			GridManager.Instance.SetTileSelectorLocked(false);
			ShowMoveRangeMarkers();
		}
	}

	private void OnPhaseExit(Enums.CombatPhase phase)
	{
		if (phase == Enums.CombatPhase.MovementPlanning)
		{
			HideMoveRangeMarkers();
		}
	}

	private void ShowMoveRangeMarkers()
	{
		foreach (var item in _nodesInMoveRange)
		{
			GameObject marker = Instantiate(_prefabMoveRangeMarker, item.WorldPos, Quaternion.identity) as GameObject;
			marker.transform.SetParent(_moveRangeMarkersParent);
		}
	}

	private void HideMoveRangeMarkers()
	{
		foreach (Transform t in _moveRangeMarkersParent.transform)
		{
			Destroy(t.gameObject);
		}
	}

	#endregion
}