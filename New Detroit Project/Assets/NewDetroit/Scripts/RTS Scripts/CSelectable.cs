using UnityEngine;
using System.Collections;

public class CSelectable : MonoBehaviour
{
	
	private Color teamColor;
    private Color origColor;
    
    private float outlineWidth;
    private Color outlineColor;

    private Transform model;

    // indicates the number of materials in its model
    private int numberOfMaterials;

	private bool selected;

    private UnitController unitReference = null;

	// Use this for initialization
	void Start ()
    {

        model = transform.FindChild("Model");
        if (model)
        {
            // if there is a "model" children in the object it is a unit
            numberOfMaterials = model.renderer.materials.Length;

            outlineWidth = model.renderer.material.GetFloat("_OutlineWidth");

            for (int i = 0; i < numberOfMaterials; i++)
                model.renderer.materials[i].SetFloat("_OutlineWidth", 0.0f);

            model.renderer.material.SetFloat("_OutlineWidth", 0.0f);

            teamColor = outlineColor = model.renderer.material.GetColor("_OutlineColor");
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
        for (int i = 0; i < numberOfMaterials; i++)
            model.renderer.materials[i].SetColor("_OutlineColor", teamColor);
    }

	public void SetSelected ()
	{
		selected = true;
        if (model)
            for (int i = 0; i < numberOfMaterials; i++)
                model.renderer.materials[i].SetFloat("_OutlineWidth", outlineWidth);
        else
		    this.renderer.material.SetColor("_DiffuseColor", teamColor);

        if (unitReference)
            unitReference.isSelected = true;
	}
	
	public void SetDeselect ()
	{
		selected = false;
        if (model)
            for (int i = 0; i < numberOfMaterials; i++)
                model.renderer.materials[i].SetFloat("_OutlineWidth", 0.0f);
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
        for (int i = 0; i < numberOfMaterials; i++)
            model.renderer.materials[i].SetColor("_OutlineColor", outlineColor);
    }

}
