using UnityEngine;
using System.Collections;

public class CSelectable : MonoBehaviour {
	
	private Color origColor;
	private Color selectColor = Color.yellow;

	private bool selected;

	// Use this for initialization
	void Start () {
		origColor = this.renderer.material.color;
				
		selected = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetSelected ()
	{
		selected = true;
		this.renderer.material.color = selectColor;
	}
	
	public void SetDeselect()
	{
		selected = false;
		this.renderer.material.color = origColor;
	}
	
	public void SetSelected(bool selected)
	{
		if (selected)
			SetSelected();
		else
			SetDeselect();
	}
	
	public bool IsSelected ()
	{
		return selected;
	}
	
	public void ResetColor ()
	{
		this.renderer.material.color = origColor;
	}
}
