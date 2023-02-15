using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region Fields

	public static UIManager Instance;

	public GameObject unitInfoPanel;
	public Image unitBackground;
	public Image unitAvatar;
	public TextMeshProUGUI unitName;
	public TextMeshProUGUI unitLevel;
	public TextMeshProUGUI unitHealth;
	public TextMeshProUGUI unitMana;

	public SpriteRenderer tileSelectorSprite;

	#endregion

	#region Unity

	private void Awake()
	{
		Instance = this;
	}

	#endregion

	#region Methods

	public void DisplayUnitInfo(Unit unit)
	{
		unitInfoPanel.SetActive(unit != null);

		if (unit != null)
		{
			unitBackground.color = unit.color;
			unitAvatar.sprite = unit.Avatar;
			unitName.text = unit.Name;
			unitLevel.text = "Lv " + unit.Level.ToString();
			unitHealth.text = $"{unit.Health} / {unit.HealthMax}";
			unitMana.text = $"{unit.Mana} / {unit.ManaMax}";
			tileSelectorSprite.color = unit.color;
		}
		else
		{
			tileSelectorSprite.color = Color.black;
		}
	}

	#endregion
}