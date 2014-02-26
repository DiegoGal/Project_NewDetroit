using UnityEngine;
using System.Collections;

public class CSelectable : MonoBehaviour
{
	
	private Color origColor;
	private Color selectColor = Color.yellow;

	private bool selected;

	// Use this for initialization
	void Start ()
    {
        if (this.renderer != null)
		    origColor = this.renderer.material.color;
		selected = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

	public void SetSelected ()
	{
		selected = true;
        if (this.renderer != null)
		    this.renderer.material.color = selectColor;
	}
	
	public void SetDeselect ()
	{
		selected = false;
        if (this.renderer != null)
		    this.renderer.material.color = origColor;
	}
	
	public void SetSelected (bool selected)
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
