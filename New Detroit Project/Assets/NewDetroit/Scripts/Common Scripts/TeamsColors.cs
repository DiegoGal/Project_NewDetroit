using UnityEngine;
using System.Collections;

public class TeamsColors : MonoBehaviour
{

    private static float inv255 = 0.003921568627451f;
    public static Color color0 = new Color(1.0f, 80.0f * inv255, 0.0f);
    public static Color color1 = new Color(0.0f, 200.0f * inv255, 1.0f);
    public static Color color2 = new Color(99.0f * inv255, 1.0f, 128.0f * inv255);
    public static Color color3 = new Color(156.0f * inv255, 0.0f, 1.0f);
    
    public static Color [] colors = { color0, color1, color2, color3 };

}
