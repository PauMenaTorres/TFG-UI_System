using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ModularText : ModularComponents
{
	public enum TextStyle
	{
		Body,
		Title
    }

	public TextStyle textRole;

	private TextMeshProUGUI textComponent;

	protected override void Awake()
	{
		textComponent = GetComponent<TextMeshProUGUI>();
		base.Awake();
	}

	public override void ApplyTheme()
	{
		base.ApplyTheme();

		if (textComponent == null)
		{
			textComponent = GetComponent<TextMeshProUGUI>();
		}

		if (currentTheme == null || useOverride)
		{
			return;
		}

		if (textRole == TextStyle.Title)
		{
			textComponent.font = currentTheme.GetTitleFont();
			textComponent.fontSize = currentTheme.headerFontSize;
			textComponent.color = currentTheme.primaryColor;
		}
		else if (textRole == TextStyle.Body)
		{
			textComponent.font = currentTheme.GetTextFont();
			textComponent.fontSize = currentTheme.baseFontSize;
			textComponent.color = currentTheme.secondaryColor;
		}
	}
}