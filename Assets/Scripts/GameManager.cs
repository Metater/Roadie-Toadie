using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int TilemapWidth = 19;

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] private SpriteRenderer fadeIn;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private float fadeInTime;

    private float realDeltaTime => (Time.deltaTime * (1 / Time.timeScale));
    private decimal realTime = 0;


    public bool Loaded { get; private set; } = false;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        Time.timeScale = 10;
        fadeIn.gameObject.SetActive(true);
        fadeIn.material.color = startColor;
    }

    private void Update()
    {
        realTime += (decimal)realDeltaTime;
        if (realTime >= 1 && !Loaded)
        {
            Loaded = true;
            Time.timeScale = 1;
            StartCoroutine(Fade());
        }
    }

    private void FixedUpdate()
    {
        Camera.main.orthographicSize = ((float)Screen.height / (float)Screen.width) * ((float)TilemapWidth / 2f);
    }

    public void Test()
    {
        Debug.Log("EEEEEE");

        // Abandoning tile rows that are really far down the level
        // Stop players from moving back too far in the level if they already passed it
        // TODO Have a higher level roadie component with more controllable stuff, and has roadie components insides
    }

    private IEnumerator Fade()
    {
        float r = fadeIn.material.color.r;
        float g = fadeIn.material.color.g;
        float b = fadeIn.material.color.b;
        float a = fadeIn.material.color.a;
        for (float t = 0.0f; t < 1.0f; t += realDeltaTime / fadeInTime)
        {
            Color newColor = new Color(Mathf.Lerp(r, endColor.r, t), Mathf.Lerp(g, endColor.g, t), Mathf.Lerp(b, endColor.b, t), Mathf.Lerp(a, endColor.a, t));
            fadeIn.color = newColor;
            yield return null;
        }
        fadeIn.gameObject.SetActive(false);
    }
}
