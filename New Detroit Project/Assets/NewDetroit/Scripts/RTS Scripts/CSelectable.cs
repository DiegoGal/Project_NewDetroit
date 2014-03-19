using UnityEngine;
using System.Collections;

public class CSelectable : MonoBehaviour
{
	
	private Color origColor;
	private Color selectColor = Color.yellow;
    
    private float outlineWidth;
    private Color outlineColor;

    private Transform model;

	private bool selected;

    private UnitController unitReference = null;

	// Use this for initialization
	void Awake ()
    {
        if (this.renderer != null)
		    origColor = this.renderer.material.color;

        model = transform.FindChild("Model");
        if (model != null)
        {
            outlineWidth = model.renderer.material.GetFloat("_OutlineWidth");
            outlineColor = model.renderer.material.GetColor("_OutlineColor");
            
            model.renderer.material.SetFloat("_OutlineWidth", 0.0f);
        }

		selected = false;

        unitReference = GetComponent<UnitController>();
	}

	public void SetSelected ()
	{
		selected = true;
        if (model != null)
            model.renderer.material.SetFloat("_OutlineWidth", outlineWidth);
        else if (this.renderer != null)
		    this.renderer.material.color = selectColor;

        if (unitReference)
            unitReference.isSelected = true;
	}
	
	public void SetDeselect ()
	{
		selected = false;
        if (model != null)
            model.renderer.material.SetFloat("_OutlineWidth", 0.0f);
        else if (this.renderer != null)
		    this.renderer.material.color = origColor;

        if (unitReference)
            unitReference.isSelected = false;
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

    public void SetOutlineColor (Color color)
    {
        outlineColor = color;
        model.renderer.material.SetColor("_OutlineColor", outlineColor);
    }

}
