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
					rectangleLabelLevel,
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
		float 	width = Screen.width/8,
				height = Screen.height/4,
				x = Screen.width - width,
				y = Screen.height - height;
		rectangleAttributes = new Rect (x, y, width, height);
		// Background for life, adrenaline, mana and skills
		width = Screen.width/3;
		x = Screen.width/2 - width/2;
		y = Screen.height - rectangleAttributes.height;
		rectangleLifeManaAdrenSkills = new Rect (x, y, width, rectangleAttributes.height);
		// Labels for heroe's attributes
		height = rectangleAttributes.height/7;
		rectangleLabelPAttack = new Rect(rectangleAttributes.x + 10, rectangleAttributes.y, rectangleAttributes.width, height);
		rectangleLabelMAttack = new Rect(rectangleAttributes.x + 10, rectangleAttributes.y + height, rectangleAttributes.width, height);
		rectangleLabelPDefense = new Rect(rectangleAttributes.x + 10, rectangleAttributes.y + 2*height, rectangleAttributes.width, height);
		rectangleLabelMDefense = new Rect(rectangleAttributes.x + 10, rectangleAttributes.y + 3*height, rectangleAttributes.width, height);
		rectangleLabelSAttack = new Rect(rectangleAttributes.x + 10, rectangleAttributes.y + 4*height, rectangleAttributes.width, height);
		rectangleLabelSMov = new Rect(rectangleAttributes.x + 10, rectangleAttributes.y + 5*height, rectangleAttributes.width, height);
		rectangleLabelLevel = new Rect(rectangleAttributes.x + 10, rectangleAttributes.y + 6*height,rectangleAttributes.width, height);
		// Bars for life, adrenaline and mana
		float 	positiveLife = (float) heroe.currentLife / heroe.maximunLife, // percentage of positive life
				positiveAdren = (float) heroe.currentAdren / heroe.adren, // percentage of positive adrenaline
				positiveMana = (float) heroe.currentMana / heroe.mana; // percentage of positive mana
		y = rectangleLifeManaAdrenSkills.y + rectangleLifeManaAdrenSkills.height/2;
		width = rectangleLifeManaAdrenSkills.width*positiveLife;
		height = rectangleLifeManaAdrenSkills.height/6;
		rectangleLife = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, width - 6*positiveLife, height - 6);
		y = y + height;
		width = rectangleLifeManaAdrenSkills.width*positiveAdren;
		rectangleAdren = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, width - 6*positiveAdren, height - 6);
		y = y + height;
		width = rectangleLifeManaAdrenSkills.width*positiveMana;
		rectangleMana = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, width - 6*positiveMana, height - 6);
		// Labels for life, adrenaline and mana
		x = rectangleLifeManaAdrenSkills.x + rectangleLifeManaAdrenSkills.width/2;
		rectangleLabelLife = new Rect(x - 40, rectangleLife.y, 80, rectangleLife.height);
		rectangleLabelAdren = new Rect(x - 40, rectangleAdren.y, 80, rectangleLife.height);
		rectangleLabelMana = new Rect(x - 40, rectangleMana.y, 80, rectangleLife.height);
		// Skills
		width = rectangleLifeManaAdrenSkills.width/3;
		height = rectangleLifeManaAdrenSkills.height/2;
		rectangleButtonSkill1 = new Rect(rectangleLifeManaAdrenSkills.x + 3, rectangleLifeManaAdrenSkills.y + 3, width - 6, height - 6);
		x = rectangleLifeManaAdrenSkills.x + width;
		rectangleButtonSkill2 = new Rect(x + 3, rectangleLifeManaAdrenSkills.y + 3, width - 6, height - 6);
		x = x + width;
		rectangleButtonSkill3 = new Rect(x + 3, rectangleLifeManaAdrenSkills.y + 3, width - 6, height - 6);
		// Select skills
		y = rectangleLifeManaAdrenSkills.y - height;
		rectangleSelectSkill1 = new Rect(rectangleButtonSkill1.x + 3, y + 3, width - 6, height - 6);
		rectangleSelectSkill2 = new Rect(rectangleButtonSkill2.x + 3, y + 3, width - 6, height - 6);
		rectangleSelectSkill3 = new Rect(rectangleButtonSkill3.x + 3, y + 3, width - 6, height - 6);
	}

	private void clickRects()
	{
		// Select new skill
		if (heroe.counterAbility > 0)
		{
			if (Input.GetMouseButtonUp(0))
			{
				Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
				if (!heroe.ability1 && rectangleSelectSkill1.Contains(mousePosition))
				{
					heroe.ability1 = true;
				}
				else if (!heroe.ability2 && rectangleSelectSkill2.Contains(mousePosition))
				{
					heroe.ability2 = true;
				}
				else if (!heroe.ability3 && heroe.level == 4 && rectangleSelectSkill3.Contains(mousePosition))
				{
					heroe.ability3 = true;
				}
			}
		}
		// Use skills
		if (Input.GetMouseButtonUp(0))
		{
			Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
			if  (heroe.ability1 && rectangleButtonSkill1.Contains(mousePosition))
			{
				heroe.UpdateState(true, false, false);
				if (heroe.type == HeroeController.TypeHeroe.Orc) ((OrcController)heroe).UpdateAnimation();
			}
			else if (heroe.ability2 && rectangleButtonSkill2.Contains(mousePosition))
			{
				heroe.UpdateState(false, true, false);
				if (heroe.type == HeroeController.TypeHeroe.Orc) ((OrcController)heroe).UpdateAnimation();
			}
			else if (heroe.ability3 && rectangleButtonSkill3.Contains(mousePosition))
			{
				heroe.UpdateState(false, false, true);
				if (heroe.type == HeroeController.TypeHeroe.Orc) ((OrcController)heroe).UpdateAnimation();
			}
		}
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
		if (heroe != null) 
		{
			GUIRects();
			clickRects();
		}
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
			GUI.Label ( rectangleLabelLevel, "Level " + heroe.level);
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
			GUI.enabled = heroe.ability1;
			GUI.Button(rectangleButtonSkill1, "Skill 1");
			GUI.enabled = heroe.ability2;
			GUI.Button(rectangleButtonSkill2, "Skill 2");
			GUI.enabled = heroe.ability3;
			GUI.Button(rectangleButtonSkill3, "Skill 3");
			GUI.enabled = true;

			if (heroe.counterAbility > 0)
			{
				if (!heroe.ability1) GUI.Button(rectangleSelectSkill1, "Select Skill 1");
				if (!heroe.ability2) GUI.Button(rectangleSelectSkill2, "Select Skill 2");
				if (heroe.level == 4)
					if (!heroe.ability3) GUI.Button(rectangleSelectSkill3, "Select Skill 3");
			}
		}
	}
}

