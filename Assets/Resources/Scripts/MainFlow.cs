using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainFlow : MonoBehaviour
{
    #region SINGLETON
    private static MainFlow instance = null;
    public static MainFlow Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public float worldHeight;
    public float worldWidth;
    public float restartAfter;

    public GameObject gameWorld;
    public GameObject gameBackgroundParticles;
    public GameObject menuBackgroundParticles;

    SpawnManager spawnManager;
    public SoundManager soundManager;
    public WallPlacing wallPlacing;
    public WallPlacing GetWallPlacingRef => wallPlacing;
    
    public Ball playerBall { get; private set; }

    public static bool isDead;
    public static bool isGameStarted;

    GameObject explosionPrefab;
    GameObject playerExplosionPrefab;

    private void Start()
    {
        isDead = false;
        LoadResources();
        SetCursorModeAndVisibility(CursorLockMode.None, true);
    }

    public void InitializeReferences()
    {
        SetGameWorld(true);
        EnableOrDisableParticles(true, false);

        UIManager.Instance.MenuVisibility(false);
        SetCursorModeAndVisibility(CursorLockMode.None, false);

        spawnManager.SpawnEnemiesOnStart();
        playerBall = GameObject.FindObjectOfType<Ball>();
        playerBall.Initialize();

        isDead = false;
        isGameStarted = true;
    }

    public void LoadResources()
    {
        explosionPrefab = Resources.Load<GameObject>("Prefabs/ExplosionEffect");
        playerExplosionPrefab = Resources.Load<GameObject>("Prefabs/PlayerExplosionEffect");

        spawnManager = GetComponent<SpawnManager>();
        spawnManager.LoadEnemies();
        soundManager.Initialize();

        wallPlacing = GetComponent<WallPlacing>();
        //wallPlacing.InitializeWallPositions();

        UIManager.Instance.Initialize();
        CameraShaker.Instance.Initialize();

        EnableOrDisableParticles(false, true);
    }

    void EnableOrDisableParticles(bool gameParticles, bool menuParticles)
    {
        SetGameBackgroundParticles(gameParticles);
        SetMenuBackgroundParticles(menuParticles);
    }

    void SetGameWorld(bool toShow)
    {
        gameWorld.SetActive(toShow);
    }

    void Update()
    {
        UIManager.Instance.Refresh();
        CameraShaker.Instance.Refresh();

        if (!isGameStarted) return;

        playerBall.Refresh();
        spawnManager.RefreshAll();
    }

    public void SlowMotion(bool activate)
    {
        if (!playerBall.gameObject.activeSelf) return;

        if(activate)
        {
            Time.timeScale = 0.3f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    public void PlayerDied()
    {
        isDead = true;
        isGameStarted = false;
        soundManager.PlayExplosionSound();
        SetTimeScale(1);
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSecondsRealtime(restartAfter);
        
        SceneManager.LoadScene(0);
    }

    public void RestartFromUI()
    {
        isDead = true;
        isGameStarted = false;
        SetTimeScale(1);
        SceneManager.LoadScene(0);
    }

    public void SetTimeScale(float timeScaleValue)
    {
        Time.timeScale = timeScaleValue;
    }

    public void SetCursorModeAndVisibility(CursorLockMode lockMode, bool isvisible)
    {
        Cursor.lockState = lockMode;
        Cursor.visible = isvisible;
    }

    public void PlayExplosionAt(Vector2 pos)
    {
        GameObject effect = Instantiate(explosionPrefab, pos, Quaternion.identity);
        Destroy(effect, 2f);
    }

    public void PlayPlayerExplosionAt(Vector2 pos)
    {
        GameObject effect = Instantiate(playerExplosionPrefab, pos, Quaternion.identity);
        Destroy(effect, 2f);
    }

    public void SetGameBackgroundParticles(bool toShow)
    {
        gameBackgroundParticles.SetActive(toShow);
    }

    public void SetMenuBackgroundParticles(bool toShow)
    {
        menuBackgroundParticles.SetActive(toShow);
    }
}
