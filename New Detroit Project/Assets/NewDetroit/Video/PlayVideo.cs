using UnityEngine;
using System.Collections;

public class PlayVideo : MonoBehaviour {

    public MovieTexture mt;
    public GameObject offline;
    public GameObject online;
    public GameObject video;
    public GameObject exit;

    AudioSource audioS;

	// Use this for initialization
	void Start () {
        //MovieTexture vt = renderer.material.mainTexture as MovieTexture;
        //((MovieTexture)GetComponent<UITexture>().mainTexture).Play();
        //vt.Play();
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
        offline.SetActive(true);
        online.SetActive(true);
        video.SetActive(true);
        exit.SetActive(true);
        gameObject.SetActive(false);
        Destroy(audioS);
    }

    public void PlayVideoNow ()
    {
        audioS = gameObject.AddComponent<AudioSource>();
        audioS.clip = mt.audioClip;

        gameObject.SetActive(true);
        offline.SetActive(false);
        online.SetActive(false);
        video.SetActive(false);
        exit.SetActive(false);
        StartCoroutine("WaitForClip");
    }
}
