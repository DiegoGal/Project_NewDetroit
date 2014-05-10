using UnityEngine;
using System.Collections;

public class CSelectable : MonoBehaviour
{
	
	private Color teamColor;
    private Color origColor;
    
    private float outlineWidth;
    private Color outlineColor;

    private Transform model;

	private bool selected;

    private UnitController unitReference = null;

	// Use this for initialization
	void Start ()
    {

        model = transform.FindChild("Model");
        if (model != null)
        {
            // if there is a "model" children in the object it is a unit
            outlineWidth = model.renderer.material.GetFloat("_OutlineWidth");
            teamColor = outlineColor = model.renderer.material.GetColor("_OutlineColor");

            model.renderer.material.SetFloat("_OutlineWidth", 0.0f);
        }
        else
        {
            // if not, it is a building
            teamColor = TeamsColors.colors[GetComponent<CTeam>().teamColorIndex];
            origColor = renderer.material.GetColor("_DiffuseColor");
        }

		selected = false;

        unitReference = GetComponent<UnitController>();
	}

    public void ResetTeamColor ()
    {
        teamColor = outlineColor = TeamsColors.colors[GetComponent<CTeam>().teamColorIndex];
    }

	public void SetSelected ()
	{
		selected = true;
        if (model != null)
            model.renderer.material.SetFloat("_OutlineWidth", outlineWidth);
        else
		    this.renderer.material.SetColor("_DiffuseColor", teamColor);

        if (unitReference)
            unitReference.isSelected = true;
	}
	
	public void SetDeselect ()
	{
		selected = false;
        if (model != null)
            model.renderer.material.SetFloat("_OutlineWidth", 0.0f);
        else
            this.renderer.material.SetColor("_DiffuseColor", origColor);

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
