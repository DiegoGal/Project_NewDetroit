using UnityEngine;
using System.Collections;

public class CameraMOBAController : MonoBehaviour
{
	public Texture2D 	backgroundHUDTexture,
						lifeTexture,
						adrenTexture,
						manaTexture;
	public HeroeController heroe;

	private Rect 	rectangleAttributes,
					rectangleLifeManaAdrenSkills,
					rectangleLabelPAttack,
					rectangleLabelMAttack,
					rectangleLabelPDefense,
					rectangleLabelMDefense,
					rectangleLabelSAttack,
					rectangleLabelSMov,
					rectangleLife,
					rectangleAdren,
					rectangleMana,
					rectangleLabelLife,
					rectangleLabelAdren,
					rectangleLabelMana,
					rectangleButtonSkill1,
					rectangleButtonSkill2,
					rectangleButtonSkill3,
					rectangleSelectSkill1,
					rectangleSelectSkill2,
					rectangleSelectSkill3;


	//-----------------------------------------------------------------------------
	private void GUIRects()
	{
		// Background for attributes
		float 	widthAttributes = Screen.width/8,
				heightAttributes = Screen.height/4,
				beginWidthAttributes = Screen.width - widthAttributes,
				beginHeightAttributes = Screen.height - heightAttributes;
		rectangleAttributes = new Rect (beginWidthAttributes, beginHeightAttributes, widthAttributes, heightAttributes);
		// Background for life, adrenaline, mana and skills
		float 	widthLifeManaAdrenSkills = Screen.width/3,
				heightLifeManaAdrenSkills = heightAttributes,
				beginWidthLifeManaAdrenSkills = Screen.width/2 - widthLifeManaAdrenSkills/2,
				beginHeightLifeManaAdrenSkills = Screen.height - heightLifeManaAdrenSkills;
		rectangleLifeManaAdrenSkills = new Rect (beginWidthLifeManaAdrenSkills, beginHeightLifeManaAdrenSkills, 
		                                              widthLifeManaAdrenSkills, heightLifeManaAdrenSkills);
		// Labels for heroe's attributes
		float heightLabelAttr = heightAttributes/6;
		rectangleLabelPAttack = new Rect(beginWidthAttributes + 10, beginHeightAttributes, widthAttributes, heightLabelAttr);
		rectangleLabelMAttack = new Rect(beginWidthAttributes + 10, beginHeightAttributes + heightLabelAttr, widthAttributes, heightLabelAttr);
		rectangleLabelPDefense = new Rect(beginWidthAttributes + 10, beginHeightAttributes + 2*heightLabelAttr, widthAttributes, heightLabelAttr);
		rectangleLabelMDefense = new Rect(beginWidthAttributes + 10, beginHeightAttributes + 3*heightLabelAttr, widthAttributes, heightLabelAttr);
		rectangleLabelSAttack = new Rect(beginWidthAttributes + 10, beginHeightAttributes + 4*heightLabelAttr, widthAttributes, heightLabelAttr);
		rectangleLabelSMov = new Rect(beginWidthAttributes + 10, beginHeightAttributes + 5*heightLabelAttr, widthAttributes, heightLabelAttr);
		// Bars for life, adrenaline and mana
		float 	positiveLife = (float) heroe.currentLife / heroe.maximunLife, // percentage of positive life
				positiveAdren = (float) heroe.currentAdren / heroe.adren, // percentage of positive adrenaline
				positiveMana = (float) heroe.currentMana / heroe.mana, // percentage of positive mana
				beginHeightLifeManaAdren = beginHeightLifeManaAdrenSkills + heightLifeManaAdrenSkills/2,
				widthLife = widthLifeManaAdrenSkills*positiveLife,
				widthAdren = widthLifeManaAdrenSkills*positiveAdren,
				widthMana = widthLifeManaAdrenSkills*positiveMana,
				heightLifeManaAdren = heightLifeManaAdrenSkills/6,
				beginHeightLife = beginHeightLifeManaAdren,
				beginHeightAdren = beginHeightLifeManaAdren + heightLifeManaAdren,
				beginHeightMana = beginHeightLifeManaAdren + 2*heightLifeManaAdren;
		rectangleLife = new Rect(beginWidthLifeManaAdrenSkills + 3, beginHeightLife + 3, widthLife - 6*positiveLife, heightLifeManaAdren - 6);
		rectangleAdren = new Rect(beginWidthLifeManaAdrenSkills + 3, beginHeightAdren + 3, widthAdren - 6*positiveAdren, heightLifeManaAdren - 6);
		rectangleMana = new Rect(beginWidthLifeManaAdrenSkills + 3, beginHeightMana + 3, widthMana - 6*positiveMana, heightLifeManaAdren - 6);
		// Labels for life, adrenaline and mana
		float beginWidthLabelLifeAdrenMana = beginWidthLifeManaAdrenSkills + widthLifeManaAdrenSkills/2;
		rectangleLabelLife = new Rect(beginWidthLabelLifeAdrenMana - 40, beginHeightLife, 80, heightLifeManaAdren);
		rectangleLabelAdren = new Rect(beginWidthLabelLifeAdrenMana - 40, beginHeightAdren, 80, heightLifeManaAdren);
		rectangleLabelMana = new Rect(beginWidthLabelLifeAdrenMana - 40, beginHeightMana, 80, heightLifeManaAdren);
		// Skills
		float 	widthSkill = widthLifeManaAdrenSkills/3,
				heightSkill = heightLifeManaAdrenSkills/2,
				beginWidthSkill1 = beginWidthLifeManaAdrenSkills,
				beginWidthSkill2 = beginWidthLifeManaAdrenSkills + widthSkill,
				beginWidthSkill3 = beginWidthLifeManaAdrenSkills + 2*widthSkill;
		rectangleButtonSkill1 = new Rect(beginWidthSkill1 + 3, beginHeightLifeManaAdrenSkills + 3, widthSkill - 6, heightSkill - 6);
		rectangleButtonSkill2 = new Rect(beginWidthSkill2 + 3, beginHeightLifeManaAdrenSkills + 3, widthSkill - 6, heightSkill - 6);
		rectangleButtonSkill3 = new Rect(beginWidthSkill3 + 3, beginHeightLifeManaAdrenSkills + 3, widthSkill - 6, heightSkill - 6);
		// Select skills
		float beginHeightSelectSkills = beginHeightLifeManaAdrenSkills - heightSkill;
		rectangleSelectSkill1 = new Rect(beginWidthSkill1 + 3, beginHeightSelectSkills + 3, widthSkill - 6, heightSkill - 6);
		rectangleSelectSkill2 = new Rect(beginWidthSkill2 + 3, beginHeightSelectSkills + 3, widthSkill - 6, heightSkill - 6);
		rectangleSelectSkill3 = new Rect(beginWidthSkill3 + 3, beginHeightSelectSkills + 3, widthSkill - 6, heightSkill - 6);
	}


	//-----------------------------------------------------------------------------
	// Use this for initialization
	void Start ()
	{
		heroe = null;
	}

	// Update is called once per frame
	void Update ()
	{
		if (heroe != null) GUIRects();
	}

	void OnGUI ()
	{
		if (heroe != null)
		{
			// Backgrounds
			GUI.DrawTexture (rectangleAttributes, backgroundHUDTexture);
			GUI.DrawTexture (rectangleLifeManaAdrenSkills, backgroundHUDTexture);
			// Label for attributes
			TextAnchor ta = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label (	rectangleLabelPAttack, "P. attack " + heroe.attackP);
			GUI.Label (	rectangleLabelMAttack, "M. attack " + heroe.attackM);
			GUI.Label (	rectangleLabelPDefense, "P. defense " + heroe.defP);
			GUI.Label (	rectangleLabelMDefense, "M. defense " + heroe.defM);
			GUI.Label (	rectangleLabelSAttack, "S. attack " + heroe.speedAtt);
			GUI.Label (	rectangleLabelSMov, "S. move " + heroe.speedMov);
			GUI.skin.label.alignment = ta;
			// Life, Mana, Adrenaline
			GUI.DrawTexture (rectangleLife, lifeTexture);
			GUI.DrawTexture (rectangleAdren, adrenTexture);
			GUI.DrawTexture (rectangleMana, manaTexture);
			// Labels for life, adrenaline and mana
			FontStyle fs = GUI.skin.label.fontStyle;
			ta = GUI.skin.label.alignment;
			GUI.skin.label.fontStyle = FontStyle.Bold;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label (rectangleLabelLife, "" + heroe.currentLife + " / " + heroe.maximunLife);
			GUI.Label (rectangleLabelAdren, "" + heroe.currentAdren + " / " + heroe.adren);
			GUI.Label (rectangleLabelMana, "" + heroe.currentMana + " / " + heroe.mana);
			GUI.skin.label.fontStyle = fs;
			GUI.skin.label.alignment = ta;
			// Skills
			int left = GUI.skin.button.border.left,
				right = GUI.skin.button.border.right,
				top = GUI.skin.button.border.top,
				bottom = GUI.skin.button.border.bottom;
			GUI.enabled = heroe.ability1;
			GUI.Button(rectangleButtonSkill1, "Skill 1");
			GUI.enabled = heroe.ability2;
			GUI.Button(rectangleButtonSkill2, "Skill 2");
			GUI.enabled = heroe.ability3;
			GUI.Button(rectangleButtonSkill3, "Skill 3");
			GUI.enabled = true;

			GUI.Button(rectangleSelectSkill1, "Select Skill 1");
			GUI.Button(rectangleSelectSkill2, "Select Skill 2");
			GUI.Button(rectangleSelectSkill3, "Select Skill 3");
		}
	}
}

