using UnityEngine;
using System.Collections;

public class MousePoint : MonoBehaviour {

    public Texture2D cursorImage;
    CursorMode cursorMode = CursorMode.Auto;
    Vector2 hotSpot = Vector2.zero;

    private int cursorWidth = 32;
    private int cursorHeight = 32;

	// Use this for initialization
	void Start () {
        Invoke("SetCustomCursor", 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    //void OnGUI()
    //{
    //    GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursorWidth, cursorHeight), cursorImage);
    //}

    //void OnMouseEnter () {
    //    Cursor.SetCursor(cursorImage, hotSpot, cursorMode);
    //}

    //void OnMouseExit () {
    //    Cursor.SetCursor(null, Vector2.zero, cursorMode);
    //}
    private void SetCustomCursor()
    {
        Cursor.SetCursor(cursorImage, hotSpot, cursorMode);
    }
}
