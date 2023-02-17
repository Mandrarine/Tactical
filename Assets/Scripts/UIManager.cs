using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region Fields

	public static UIManager Instance;

	public GameObject unitInfoPanel;
	public Image[] unitBackgrounds;
	public Image unitAvatar;
	public TextMeshProUGUI unitName;
	public TextMeshProUGUI unitLevel;
	public TextMeshProUGUI unitHealth;
	public TextMeshProUGUI unitMana;

	public SpriteRenderer tileSelectorSpriteEdge;
	public SpriteRenderer tileSelectorSpriteMiddle;

	public Color colorTileSelectorEdgeDefault;
	public Color colorTileSelectorMiddleDefault;

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
			foreach (Image image in unitBackgrounds)
			{
				image.color = unit.color;
			}

			unitAvatar.sprite = unit.Avatar;
			unitName.text = unit.Name;
			unitLevel.text = "Lv " + unit.Level.ToString();
			unitHealth.text = $"{unit.Health} / {unit.HealthMax}";
			unitMana.text = $"{unit.Mana} / {unit.ManaMax}";

			tileSelectorSpriteEdge.color = unit.color;
			tileSelectorSpriteMiddle.color = new Color(unit.color.r, unit.color.g, unit.color.b, 0.5f);
		}
		else
		{
			tileSelectorSpriteEdge.color = colorTileSelectorEdgeDefault;
			tileSelectorSpriteMiddle.color = colorTileSelectorMiddleDefault;
		}
	}

	#endregion
}