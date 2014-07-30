using UnityEngine;
using System.Collections;

public class PlayVideo : MonoBehaviour {

    public MovieTexture mt;
    public GameObject online;
    public GameObject video;
    public GameObject exit;
    public UILabel texto;

    AudioSource audioS;

	// Use this for initialization
	void Start () {
        
	}

	// Update is called once per frame
	void Update () {
	
	}

    public void Disable()
    {      
        mt.Stop();
        Destroy(audioS);
        
    }

    private IEnumerator WaitForClip()
    {
        mt.Play();
        audioS.Play();
        while (mt.isPlaying)
            yield return new WaitForSeconds(0.1f);
        //Disable();
        online.SetActive(true);
        video.SetActive(true);
        exit.SetActive(true);
        gameObject.SetActive(false);
        Destroy(audioS);
    }

    public void PlayVideoNow()
    {
        if ( mt != null )
        {
            audioS = gameObject.AddComponent<AudioSource>();
            audioS.clip = mt.audioClip;

            gameObject.SetActive(true);
            online.SetActive(false);
            video.SetActive(false);
            exit.SetActive(false);
            StartCoroutine("WaitForClip");
        }
    }

    public void UpdateText()
    {
        texto.text = "C:> Welcome to the madness game Mutant Meat City. \n\n" +
        "        press play online to select a room and join a game! \n\n" +
        "        or play the video. It's amazing! \n\n" +
        "        or exit the game... wait! are you sure?? :(";
        texto.UpdateNGUIText();
    }
}
