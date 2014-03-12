using UnityEngine;
using System.Collections;

public class CameraMOBAController : MonoBehaviour
{
	public Texture2D backgroundHUDTexture;
	public HeroeController heroe;
	
	
	// Use this for initialization
	void Start ()
	{
		heroe = null;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnGUI ()
	{
		if (heroe != null)
		{
			// Backgrounds
			float 	widthAttributes = Screen.width/8,
					heightAttributes = Screen.height/4,
					beginWidthAttributes = Screen.width - widthAttributes,
					beginHeightAttributes = Screen.height - heightAttributes;
					
			Rect rectangleAttributes = new Rect (beginWidthAttributes, beginHeightAttributes, widthAttributes, heightAttributes);
			
			float 	widthLifeManaAdrenSkills = Screen.width/3,
					heightLifeManaAdrenSkills = heightAttributes,
					beginWidthLifeManaAdrenSkills = Screen.width/2 - widthLifeManaAdrenSkills/2,
					beginHeightLifeManaAdrenSkills = Screen.height - heightLifeManaAdrenSkills;
					
			Rect rectangleLifeManaAdrenSkills = new Rect (beginWidthLifeManaAdrenSkills, beginHeightLifeManaAdrenSkills, 
													widthLifeManaAdrenSkills, heightLifeManaAdrenSkills);
													
			GUI.DrawTexture (rectangleAttributes, backgroundHUDTexture);
			GUI.DrawTexture (rectangleLifeManaAdrenSkills, backgroundHUDTexture);
			// Attributes
			float heightLabelAttr = heightAttributes/6;
			
			GUI.Label (	new Rect (	beginWidthAttributes + 10, beginHeightAttributes, widthAttributes, heightLabelAttr), 
									"P. attack - " + heroe.attackP);
			GUI.Label (	new Rect (	beginWidthAttributes + 10, beginHeightAttributes + heightLabelAttr, widthAttributes, heightLabelAttr), 
									"M. attack - " + heroe.attackM);
			GUI.Label (	new Rect (	beginWidthAttributes + 10, beginHeightAttributes + 2*heightLabelAttr, widthAttributes, heightLabelAttr), 
									"P. defense - " + heroe.defP);
			GUI.Label (	new Rect (	beginWidthAttributes + 10, beginHeightAttributes + 3*heightLabelAttr, widthAttributes, heightLabelAttr), 
									"M. defense - " + heroe.defM);
			GUI.Label (	new Rect (	beginWidthAttributes + 10, beginHeightAttributes + 4*heightLabelAttr, widthAttributes, heightLabelAttr), 
									"S. attack - " + heroe.speedAtt);
			GUI.Label (	new Rect (	beginWidthAttributes + 10, beginHeightAttributes + 5*heightLabelAttr, widthAttributes, heightLabelAttr), 
									"P. attack - " + heroe.speedMov);
			// Life, Mana, Adrenaline and abilities
			float 	positiveLife = (float) heroe.currentLife / heroe.maximunLife, // percentage of positive life
					positiveAdren = (float) heroe.currentAdren / heroe.adren, // percentage of positive adrenaline
					positiveMana = (float) heroe.currentMana / heroe.mana, // percentage of positive mana
					
					beginHeightLifeManaAdren = beginHeightLifeManaAdrenSkills + heightLifeManaAdrenSkills/2,
					widthLife = (widthLifeManaAdrenSkills - 6)*positiveLife,
					widthAdren = (widthLifeManaAdrenSkills - 6)*positiveAdren,
					widthMana = (widthLifeManaAdrenSkills - 6)*positiveMana,
					heightLifeManaAdren = heightLifeManaAdrenSkills/6;
					
			Rect 	rectangleLife = new Rect(	beginWidthLifeManaAdrenSkills + 3, beginHeightLifeManaAdren + 3, 
												widthLife, heightLifeManaAdren - 6),
					rectangleAdren = new Rect(	beginWidthLifeManaAdrenSkills + 3, beginHeightLifeManaAdren + heightLifeManaAdren + 3, 
			                          widthAdren, heightLifeManaAdren - 6),
					rectangleMana = new Rect(	beginWidthLifeManaAdrenSkills+ 3, beginHeightLifeManaAdren + 2*heightLifeManaAdren + 3, 
			                         			widthMana, heightLifeManaAdren - 6);
			                         			
			GUI.DrawTexture (rectangleLife, heroe.textureLifePositive);
			GUI.DrawTexture (rectangleAdren, heroe.textureAdrenPositive);
			GUI.DrawTexture (rectangleMana, heroe.textureManaPositive);
		}
	}
}

