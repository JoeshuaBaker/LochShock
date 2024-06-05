using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    public Sprite[] sprites;
    public float timePerFrame = 1f/30f;
    public bool loop = true;
    public bool destroyOnEnd = false;
    public string spritePath = "Sprites/IntroAnimationFrames";

    public int frame = 0;
    public string frameName = "";
    private Image image;
    private float time = 0;
    public bool playing = false;

    void Awake()
    {
        playing = false;
        image = GetComponent<Image>();
        sprites = Resources.LoadAll<Sprite>(spritePath);
    }

    private void Reset()
    {
        sprites = Resources.LoadAll<Sprite>(spritePath);
    }

    void Update()
    {
        if(playing)
        {
            if (!loop && frame == sprites.Length) return;
            time += Time.deltaTime;
            if (time < timePerFrame) return;
            image.sprite = sprites[frame];
            frameName = image.sprite.name;
            time %= timePerFrame;
            frame++;
            if (frame >= sprites.Length)
            {
                if (loop) frame = 0;
                if (destroyOnEnd) Destroy(gameObject);
            }
        }
    }
}