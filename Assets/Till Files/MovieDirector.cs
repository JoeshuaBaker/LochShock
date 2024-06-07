using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MovieDirector : MonoBehaviour
{
    public GameObject titleUiContainer;
    public GameObject tutorialWindow;
    public string gameSceneName;
    public Canvas canvasVar;
    public ImageAnimation ImageAnimation;
    public GameObject VideoPlayerObject;
    public long holdFrame = 77;
    public long LogoSFXFrame = 45;

    private VideoPlayer Video;
    private bool check = true;
    private bool check2 = true;


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        ImageAnimation.playing = false;
        canvasVar.enabled = false;
        Video = VideoPlayerObject.gameObject.GetComponent<VideoPlayer>();
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Video.frame == LogoSFXFrame)
        {
            PlaySound(3);
        }

        if(Video.frame == 110)
        {
            PlaySound(1);
        }

        if (Video.frame >= 140 || Video.enabled == false)
        {

            Video.enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;

            canvasVar.enabled = true;            
            ImageAnimation.playing = true;               

            if (ImageAnimation.frame == holdFrame)
            {
                ImageAnimation.playing = false;
                if(titleUiContainer != null)
                {
                    titleUiContainer.SetActive(true);
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ImageAnimation.frame += 1;
                    ImageAnimation.playing = true;
                    PlaySound(2);
                }
            }

            if(ImageAnimation.frame == ImageAnimation.sprites.Length)
            {
                titleUiContainer.SetActive(false);
                tutorialWindow.SetActive(true);
                if(Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //Audio Section
                    AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
                    LoadGame();
                }
                //PlaySound(2);
                //Invoke("RestartScene", 3.0f);
            }
        }
        //Debug.Log(Video.frame);
        //Debug.Log(ImageAnimation.frame);
    }

    void PlaySound(int stage)
    {
        switch (stage)
        {
            case 1:
                if (!check) { break; }
                AkSoundEngine.PostEvent("PlayIntroStart", this.gameObject);
                AkSoundEngine.PostEvent("PlayIntroDrone", this.gameObject);
                check = false;
                break;
            case 2:
                if (!check2) { break; }
                AkSoundEngine.PostEvent("PlayIntroLaugh", this.gameObject);
                check2 = false;
                break;
            case 3:
                AkSoundEngine.PostEvent("PlayLogo", this.gameObject);
                break;
            case 4:
                AkSoundEngine.PostEvent("StopAll", this.gameObject);
                break;
        }
        return;
    }

    void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
        return;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
