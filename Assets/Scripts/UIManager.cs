using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region Attributes

	public static UIManager Instance;

	public Image unitAvatar;
	public GameObject unitInfoPanel;
	public TextMeshProUGUI unitName;
	public TextMeshProUGUI unitHealth;

	#endregion

	#region Initialization

	private void Awake()
	{
		Instance = this;
	}

	#endregion

	#region Logic

	public void DisplayUnitInfo(Unit unit)
	{
		unitInfoPanel.SetActive(unit != null);

		if (unit != null)
		{
			unitAvatar.sprite = unit.Avatar;
			unitName.text = unit.Name;
			unitHealth.text = $"{unit.Health} / {unit.HealthMax}";
		}
	}

	#endregion
}