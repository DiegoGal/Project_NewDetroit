using UnityEngine;
using System.Collections;

public class CBasicAttributesHero : CLife {

	// Adren variables
	private int maximunAdren = 100;
	private int currentAdren;

	// Mana variables
	private int maximunMana = 100;
	private int currentMana;

	// Deffense variables
	private int deffenseMagic = 10;
	private int deffensePhysic = 10;

	//Level variable
	private int level = 1;

	// GUI interface life, aden, mana and level
	private Rect 	rectanglePositiveLife,
					rectangleNegativeLife,
					rectanglePositiveAdren,
					rectangleNegativeAdren,
					rectanglePositiveMana,
					rectangleNegativeMana,
					rectangleLevel;
	public Texture2D 	textureLifePositive, textureLifeNegative,
						textureAdrenPositive, textureAdrenNegative,
						textureManaPositive, textureManaNegative,
						textureBackground;


	//-------------------------------------------------------
	// Use this for initialization
	public virtual void Start () {
		base.Start();

		currentAdren = maximunAdren;
		currentMana = maximunMana;
	}

	public virtual void Update()
	{
		GUIRects();
	}

	public virtual void OnGUI()
	{
		GUI.DrawTexture(rectanglePositiveLife, textureLifePositive);
		GUI.DrawTexture(rectangleNegativeLife, textureLifeNegative);
		GUI.DrawTexture(rectanglePositiveAdren, textureAdrenPositive);
		GUI.DrawTexture(rectangleNegativeAdren, textureAdrenNegative);
		GUI.DrawTexture(rectanglePositiveMana, textureManaPositive);
		GUI.DrawTexture(rectangleNegativeMana, textureManaNegative);
		GUI.DrawTexture(rectangleLevel, textureBackground);
		
		FontStyle fs = GUI.skin.label.fontStyle;
		TextAnchor ta = GUI.skin.label.alignment;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.Label(rectangleLevel, "" + level);
		GUI.skin.label.fontStyle = fs;
		GUI.skin.label.alignment = ta;
	}


	//-------------------------------------------------------
	private void GUIRects()
	{
		float 	distance = Vector3.Distance (transform.position, Camera.main.transform.position), // real distance from camera
		lengthLifeAdrenMana = this.GetComponent<ThirdPersonCamera> ().distance / distance, // percentage of the distance
		widthAll = Screen.width / 10,
		widthHalf = widthAll / 2,
		positiveLife = (float) currentLife / maximunLife, // percentage of positive life
		positiveAdren = (float) currentAdren / maximunAdren, // percentage of positive adrenaline
		positiveMana = (float) currentMana / maximunMana; // percentage of positive mana
		// Life
		Vector3 posScene = new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z),
		posSceneEnd = new Vector3 (transform.position.x, transform.position.y + 1.8f, transform.position.z),
		pos = Camera.main.WorldToScreenPoint (posScene),
		posEnd = Camera.main.WorldToScreenPoint (posSceneEnd);
		
		float 	x = pos.x - widthHalf * lengthLifeAdrenMana,
		y = Screen.height - pos.y,
		width = widthAll * positiveLife * lengthLifeAdrenMana,
		height = (pos.y - posEnd.y) * lengthLifeAdrenMana;	
		rectanglePositiveLife = new Rect (x, y, width, height);
		
		x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveLife * lengthLifeAdrenMana;
		width = widthAll * (1 - positiveLife) * lengthLifeAdrenMana;
		rectangleNegativeLife = new Rect (x, y, width, height);
		// Adrenaline
		posScene = new Vector3 (transform.position.x, transform.position.y + 1.78f, transform.position.z);
		posSceneEnd = new Vector3 (transform.position.x, transform.position.y + 1.68f, transform.position.z);
		pos = Camera.main.WorldToScreenPoint (posScene);
		posEnd = Camera.main.WorldToScreenPoint (posSceneEnd);
		
		x = pos.x - widthHalf * lengthLifeAdrenMana;
		y = Screen.height - pos.y;
		width = widthAll * positiveAdren * lengthLifeAdrenMana;
		height = (pos.y - posEnd.y) * lengthLifeAdrenMana;
		rectanglePositiveAdren = new Rect (x, y, width, height);
		
		x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveAdren * lengthLifeAdrenMana;
		width = widthAll * (1 - positiveAdren) * lengthLifeAdrenMana;
		rectangleNegativeAdren = new Rect (x, y, width, height);
		// Mana
		posScene = new Vector3 (transform.position.x, transform.position.y + 1.66f, transform.position.z);
		posSceneEnd = new Vector3 (transform.position.x, transform.position.y + 1.56f, transform.position.z);
		pos = Camera.main.WorldToScreenPoint (posScene);
		posEnd = Camera.main.WorldToScreenPoint (posSceneEnd);
		
		x = pos.x - widthHalf * lengthLifeAdrenMana;
		y = Screen.height - pos.y;
		width = widthAll * positiveMana * lengthLifeAdrenMana;
		height = (pos.y - posEnd.y) * lengthLifeAdrenMana;
		rectanglePositiveMana = new Rect (x, y, width, height);
		
		x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveMana * lengthLifeAdrenMana;
		width = widthAll * (1 - positiveMana) * lengthLifeAdrenMana;
		rectangleNegativeMana = new Rect (x, y, width, height);
		// Level
		posScene = new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z);
		posSceneEnd = new Vector3 (transform.position.x, transform.position.y + 1.56f, transform.position.z);
		pos = Camera.main.WorldToScreenPoint (posScene);
		posEnd = Camera.main.WorldToScreenPoint (posSceneEnd);

		x = pos.x - (widthHalf + 22) * lengthLifeAdrenMana;
		y = rectanglePositiveLife.y;
		width = 20 * lengthLifeAdrenMana;
		height = (rectanglePositiveMana.y - rectanglePositiveLife.y) + rectanglePositiveMana.height;
		rectangleLevel = new Rect (x, y, width, height);
		
	}
	
	
	//-------------------------------------------------------
	
	
	[RPC]
	public void UpdateAdren(int amount)
	{
		currentAdren += amount;
		if (currentAdren < 0) currentAdren = 0;
		if (currentAdren > maximunAdren) currentAdren = maximunAdren;
	}
	
	[RPC]
	public void UpdateMana(int amount)
	{
		currentMana += amount;
		if (currentMana < 0) currentMana = 0;
		if (currentMana > maximunMana) currentMana = maximunMana;
	}


	//-------------------------------------------------------
	public bool consumeAdren(int adren)
	{
		if (adren > currentAdren) 
			return false;
		else
		{
			photonView.RPC("UpdateAdren", PhotonTargets.All, -adren);
//			currentAdren -= adren;
			return true;
		}
	}

	public bool consumeMana(int mana)
	{
		if (mana > currentMana) 
			return false;
		else
		{
			photonView.RPC("UpdateMana", PhotonTargets.All, -mana);
//			currentMana -= mana;
			return true;
		}
	}

	public void recoverAdren(int adren)
	{
		if (currentAdren < maximunAdren)
		{
			photonView.RPC("UpdateAdren", PhotonTargets.All, adren);
		}
//		currentAdren += adren;
//		currentAdren = Mathf.Min(maximunAdren, currentAdren);
	}

	public void recoverMana(int mana)
	{
		if (currentMana < maximunMana)
		{
			photonView.RPC("UpdateMana", PhotonTargets.All, mana);
		}
//		currentMana += mana;
//		currentMana = Mathf.Min(maximunMana, currentMana);
	}

	public void levelUp()
	{
		level ++;
	}


	//-------------------------------------------------------
	public void setMaximunAdren(int adren) { maximunAdren = adren; }
	public int getMaximunAdren() { return maximunAdren; }
	public void setMaximunMana(int mana) { maximunMana = mana; }
	public int getMaximunMana() { return maximunMana; }
	public void setCurrentAdren(int adren) { currentAdren = adren; }
	public int getCurrentAdren() { return currentAdren; }
	public void setCurrentMana(int mana) { currentMana = mana; }
	public int getCurrentMana() { return currentMana; }
	public void setLevel(int level) { this.level = level; }
	public int getLevel() { return level; }
	public void setDeffenseMagic(int def) { deffenseMagic = def; }
	public int getDeffenseMagic() { return deffenseMagic; }
	public void setDeffensePhysic(int def) { deffensePhysic = def; }
	public int getDeffensePhysic() { return deffensePhysic; }

}
