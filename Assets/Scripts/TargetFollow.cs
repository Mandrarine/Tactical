using UnityEngine;

public class TargetFollow : MonoBehaviour
{
	#region Enumerations

	public enum FollowMode
	{
		INSTANT,
		MOVE_TOWARDS,
		LERP
	}

	#endregion

	#region Attributes

	[SerializeField] private Transform _target;
	[SerializeField] private FollowMode _followMode = FollowMode.INSTANT;
	[SerializeField] private bool _followX = true;
	[SerializeField] private bool _followY = true;
	[SerializeField] private bool _followZ = true;
	[SerializeField] private Vector3 _offset;
	[SerializeField] private float _interpolationSpeed = 1.0f;
	[SerializeField] private Vector2 limitsX;
	[SerializeField] private Vector2 limitsZ;

	#endregion

	#region Properties

	public Transform Target
	{
		get => _target;
		set => _target = value;
	}

	#endregion

	#region Updates

	private void Update()
	{
		if (!_target)
			return;

		float lPosX = (_followX ? _target.position.x : 0) + _offset.x;
		float lPosY = (_followY ? _target.position.y : 0) + _offset.y;
		float lPosZ = (_followZ ? _target.position.z : 0) + _offset.z;

		lPosX = Mathf.Clamp(lPosX, limitsX.x, limitsX.y);
		lPosZ = Mathf.Clamp(lPosZ, limitsZ.x, limitsZ.y);

		transform.position = _followMode switch
		{
			FollowMode.INSTANT => new Vector3(lPosX, lPosY, lPosZ),
			FollowMode.MOVE_TOWARDS => Vector3.MoveTowards(transform.position, new Vector3(lPosX, lPosY, lPosZ), Time.deltaTime * _interpolationSpeed),
			FollowMode.LERP => Vector3.Lerp(transform.position, new Vector3(lPosX, lPosY, lPosZ), Time.deltaTime * _interpolationSpeed),
			_ => transform.position
		};
	}

	#endregion

	#region Logic

	public void SetFollowMode(FollowMode pMode)
	{
		_followMode = pMode;
	}

	#endregion
}