
using UnityEngine;
using System.Collections;
using System;

public class CameraMOBAController : MonoBehaviour
{
	public Texture2D 	backgroundHUDTexture,
						lifeTexture,
						adrenTexture,
						manaTexture;

    public GameObject hero;
    private AttributesHero attHero;
    private StateHero stateHero;
    private float   distanceBack = 10, 
                    distanceHeight = 5;

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


	private void InitGUIRects()
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
        float   positiveLife = (float)attHero.currentLife / attHero.maximunLife, // percentage of positive life
                positiveAdren = (float)attHero.getCurrentAdren() / attHero.getMaximunAdren(), // percentage of positive adrenaline
                positiveMana = (float)attHero.getCurrentMana() / attHero.getMaximunMana(); // percentage of positive mana

		y = rectangleLifeManaAdrenSkills.y + rectangleLifeManaAdrenSkills.height / 2;
		width = rectangleLifeManaAdrenSkills.width * positiveLife;
		height = rectangleLifeManaAdrenSkills.height / 6;
		float 	widthRect = width - 6 * positiveLife,
				heightRect = height - 6;
		rectangleLife = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, widthRect, heightRect);
		y = y + height;
		width = rectangleLifeManaAdrenSkills.width*positiveAdren;
		widthRect = width - 6 * positiveAdren;
		heightRect = height - 6;
		rectangleAdren = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, widthRect, heightRect);
		y = y + height;
		width = rectangleLifeManaAdrenSkills.width*positiveMana;
		widthRect = width - 6 * positiveMana;
		heightRect = height - 6;
		rectangleMana = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, widthRect, heightRect);

		// Labels for life, adrenaline and mana
		x = rectangleLifeManaAdrenSkills.x + rectangleLifeManaAdrenSkills.width/2;
		rectangleLabelLife = new Rect(x - 40, rectangleLife.y, 80, rectangleLife.height);
		rectangleLabelAdren = new Rect(x - 40, rectangleAdren.y, 80, rectangleLife.height);
		rectangleLabelMana = new Rect(x - 40, rectangleMana.y, 80, rectangleLife.height);

		// Skills
		width = rectangleLifeManaAdrenSkills.width/3;
		height = rectangleLifeManaAdrenSkills.height/2;
		widthRect = width - 6;
		heightRect = height - 6;
		rectangleButtonSkill1 = new Rect(rectangleLifeManaAdrenSkills.x + 3, rectangleLifeManaAdrenSkills.y + 3, widthRect, heightRect);
		x = rectangleLifeManaAdrenSkills.x + width;
		rectangleButtonSkill2 = new Rect(x + 3, rectangleLifeManaAdrenSkills.y + 3, widthRect, heightRect);
		x = x + width;
		rectangleButtonSkill3 = new Rect(x + 3, rectangleLifeManaAdrenSkills.y + 3, widthRect, heightRect);

		// Select skills
		y = rectangleLifeManaAdrenSkills.y - height;
		rectangleSelectSkill1 = new Rect(rectangleButtonSkill1.x + 3, y + 3, width - 6, height - 6);
		rectangleSelectSkill2 = new Rect(rectangleButtonSkill2.x + 3, y + 3, width - 6, height - 6);
		rectangleSelectSkill3 = new Rect(rectangleButtonSkill3.x + 3, y + 3, width - 6, height - 6);
	}


	//-----------------------------------------------------------------------------


    private void UpdateGUIRects()
    {
        // Bars for life, adrenaline and mana
        float positiveLife = (float)attHero.currentLife / attHero.maximunLife, // percentage of positive life
                positiveAdren = (float)attHero.getCurrentAdren() / attHero.getMaximunAdren(), // percentage of positive adrenaline
                positiveMana = (float)attHero.getCurrentMana() / attHero.getMaximunMana(); // percentage of positive mana

        // Life
        float y = rectangleLifeManaAdrenSkills.y + rectangleLifeManaAdrenSkills.height / 2;
        float width = rectangleLifeManaAdrenSkills.width * positiveLife;
        float height = rectangleLifeManaAdrenSkills.height / 6;
        float widthRect = width - 6 * positiveLife;
        float heightRect = height - 6;
        rectangleLife = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, widthRect, heightRect);

        // Adren
        y = y + height;
        width = rectangleLifeManaAdrenSkills.width * positiveAdren;
        widthRect = width - 6 * positiveAdren;
        heightRect = height - 6;
        rectangleAdren = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, widthRect, heightRect);

        // Mana
        y = y + height;
        width = rectangleLifeManaAdrenSkills.width * positiveMana;
        widthRect = width - 6 * positiveMana;
        heightRect = height - 6;
        rectangleMana = new Rect(rectangleLifeManaAdrenSkills.x + 3, y + 3, widthRect, heightRect);
    }


    //-----------------------------------------------------------------------------


	void Start ()
	{
        // Components from hero
        attHero = hero.GetComponent<AttributesHero>();
        stateHero = hero.GetComponent<StateHero>();

        // Rects that represent the MOBA's GUI
        InitGUIRects();

        // Hide cursor
        Screen.showCursor = false;
	}

	void Update ()
	{
        UpdateGUIRects();

        Camera.main.transform.position = hero.transform.position + (hero.transform.forward * -distanceBack) + (hero.transform.up * distanceHeight);
        transform.LookAt(hero.transform);
       
	}

	void OnGUI ()
	{
        // Backgrounds
        GUI.DrawTexture(rectangleAttributes, backgroundHUDTexture);
        GUI.DrawTexture(rectangleLifeManaAdrenSkills, backgroundHUDTexture);

        // Label for attributes
        TextAnchor ta = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        GUI.Label(rectangleLabelPAttack, "P. attack " + attHero.getAttackPhysic());
        GUI.Label(rectangleLabelMAttack, "M. attack " + attHero.getAttackMagic());
        GUI.Label(rectangleLabelPDefense, "P. defense " + attHero.getDeffensePhysic());
        GUI.Label(rectangleLabelMDefense, "M. defense " + attHero.getDeffenseMagic());
        GUI.Label(rectangleLabelSAttack, "S. attack " + Math.Round(attHero.getSpeedAttack(), 2));
        GUI.Label(rectangleLabelSMov, "S. walk " + attHero.getSpeedWalk());
        GUI.Label(rectangleLabelLevel, "Level " + attHero.getLevel());


        GUI.skin.label.alignment = ta;

        // Life, Mana, Adrenaline
        GUI.DrawTexture(rectangleLife, lifeTexture);
        GUI.DrawTexture(rectangleAdren, adrenTexture);
        GUI.DrawTexture(rectangleMana, manaTexture);

        // Labels for life, adrenaline and mana
        FontStyle fs = GUI.skin.label.fontStyle;
        ta = GUI.skin.label.alignment;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(rectangleLabelLife, "" + attHero.currentLife + " / " + attHero.maximunLife);
        GUI.Label(rectangleLabelAdren, "" + attHero.getCurrentAdren() + " / " + attHero.getMaximunAdren());
        GUI.Label(rectangleLabelMana, "" + attHero.getCurrentMana() + " / " + attHero.getMaximunMana());
        GUI.skin.label.fontStyle = fs;
        GUI.skin.label.alignment = ta;

        // Skills
        GUI.enabled = attHero.getUseSkill1();
        GUI.Button(rectangleButtonSkill1, "Skill 1");
        GUI.enabled = attHero.getUseSkill2();
        GUI.Button(rectangleButtonSkill2, "Skill 2");
        GUI.enabled = attHero.getUseSkill3();
        GUI.Button(rectangleButtonSkill3, "Skill 3");
        GUI.enabled = true;

        // Cooldowns
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 20;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        if (attHero.getCurrentCooldown1() > 0)
        {
            GUI.DrawTexture(rectangleButtonSkill1, backgroundHUDTexture);
            GUI.Label(rectangleButtonSkill1, "" + (int)(attHero.getCurrentCooldown1()), style);
        }
        if (attHero.getCurrentCooldown2() > 0)
        {
            GUI.DrawTexture(rectangleButtonSkill2, backgroundHUDTexture);
            GUI.Label(rectangleButtonSkill2, "" + (int)(attHero.getCurrentCooldown2()), style);
        }
        if (attHero.getCurrentCooldown3() > 0)
        {
            GUI.DrawTexture(rectangleButtonSkill3, backgroundHUDTexture);
            GUI.Label(rectangleButtonSkill3, "" + (int)(attHero.getCurrentCooldown3()), style);
        }

        // Unlock abilities
        if (attHero.getLevel() > 1 && attHero.getLevel() - 1 - stateHero.GetCountSkills() > 0)
        {
            if (!attHero.getUseSkill1()) GUI.Button(rectangleButtonSkill1, "Alt + 1");
            if (!attHero.getUseSkill2()) GUI.Button(rectangleButtonSkill2, "Alt + 2");
            if (attHero.getLevel() == 4)
                if (!attHero.getUseSkill3()) GUI.Button(rectangleButtonSkill3, "Alt + 3");
        }
	}


    //----------------------------------------------------------------------------------------------


    public float GetDistanceBack() { return distanceBack; }
    public float GetDistanceHeight() { return distanceHeight; }
}

