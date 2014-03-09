using UnityEngine;
using System.Collections;

public class CameraMOBAController : MonoBehaviour
{
	public Texture2D backgroundHUDTexture;	
	
	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnGUI ()
	{
		float 	widthAttributes = Screen.width / 8,
				heightAttributes = Screen.height / 4;
		Rect rectangleAttributes = new Rect (Screen.width - widthAttributes, Screen.height - heightAttributes, widthAttributes, heightAttributes);
		
		float 	widthLifeManaAdren = Screen.width / 3,
				heightLifeManaAdren = heightAttributes;
		Rect rectangleLifeManaAdren = new Rect (Screen.width / 2 - widthLifeManaAdren / 2, Screen.height - heightLifeManaAdren, 
		                                        widthLifeManaAdren, heightLifeManaAdren);
		
		GUI.DrawTexture (rectangleAttributes, backgroundHUDTexture);
		GUI.DrawTexture (rectangleLifeManaAdren, backgroundHUDTexture);
	}
}

