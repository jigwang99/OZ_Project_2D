using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum Result { None, Victory, Defeat }
    public Result GameResult {  get; private set; } = Result.None;

    public bool IsGameOver => GameResult != Result.None;

    public event Action<Result> OnGameOver;

    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private float startDelay = 1f;

    private float checkTimer;
    private float startTimer;
    private bool ready;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Time.timeScale = 1.0f;
        Castle.ActiveCastle.Clear();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTimer = startDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver)
            return;

        if(!ready)
        {
            startTimer -= Time.deltaTime;
            if(startTimer <= 0f && Castle.CountCastles(FactionType.Player) > 0 && Castle.CountCastles(FactionType.Enemy) > 0)
                ready = true;
            return;
        }
        
        checkTimer -= Time.deltaTime;
        if (checkTimer > 0f)
            return;
        checkTimer = checkInterval;

        bool playerAlive = Castle.CountCastles(FactionType.Player) > 0;
        bool enemyAlive = Castle.CountCastles(FactionType.Enemy) > 0;

        if (!enemyAlive)
            EndGame(Result.Victory);
        else if(!playerAlive)
            EndGame(Result.Defeat);
    }
    private void EndGame(Result result)
    {
        GameResult = result;
        Time.timeScale = 0f;
        OnGameOver?.Invoke(result);
    }
}
