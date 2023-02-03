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

    public Transform Target { get; set; }
    [SerializeField] private FollowMode _followMode = FollowMode.INSTANT;
    [SerializeField] private bool _followX = true;
    [SerializeField] private bool _followY = true;
    [SerializeField] private bool _followZ = true;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _interpolationSpeed = 1.0f;

    #endregion

    #region Updates

    private void Update()
    {
        if (!Target)
            return;

        float lPosX = (_followX ? Target.position.x : 0) + _offset.x;
        float lPosY = (_followY ? Target.position.y : 0) + _offset.y;
        float lPosZ = (_followZ ? Target.position.z : 0) + _offset.z;

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