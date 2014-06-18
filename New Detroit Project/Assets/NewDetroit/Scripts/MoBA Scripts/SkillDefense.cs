using UnityEngine;
using System.Collections;

public class SkillDefense : Photon.MonoBehaviour {

	private GameObject owner;
	private int defP;	// Physic deffense
	private int defM;	// Magic deffense
	
	
	//-----------------------------------------------------
	
	
	public void UpDef()
	{
		photonView.RPC("UpDeffense", PhotonTargets.All, owner.name);
	}
	
	public void DownDef()
	{
		photonView.RPC("DownDeffense", PhotonTargets.All, owner.name);
	}
	
	
	//----------------------------------------------------
	
	
	[RPC]
	public void UpDeffense(string name)	
	{
		GameObject robot = GameObject.Find(name);
		CBasicAttributesHero cbah = robot.GetComponent<CBasicAttributesHero>();
		cbah.setDeffenseMagic(cbah.getDeffenseMagic() + 50);
		cbah.setDeffensePhysic(cbah.getDeffensePhysic() + 50);
	}
	
	[RPC]
	public void DownDeffense(string name)	
	{
		GameObject robot = GameObject.Find(name);
		CBasicAttributesHero cbah = robot.GetComponent<CBasicAttributesHero>();
		cbah.setDeffenseMagic(cbah.getDeffenseMagic() - 50);
		cbah.setDeffensePhysic(cbah.getDeffensePhysic() - 50);
		
	}
	
	
	//--------------------------------------------------
	
	
	public int getDefP() { return defP; }
	public void setDefP(int def) { defP = def; }
	public int getDefM() { return defM; }	
	public void setDefM(int def) { defM = def; }
	public void setOwner(GameObject owner) { this.owner = owner; }
	
}
