using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using JLProject.Spline;
using System;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField]
    private int _health;
    [SerializeField]
    private Text _UIHealth;
    [SerializeField]
    private Text _UIRound;
    [SerializeField]
    private Text _UIFunds;
    [SerializeField]
    private GameObject _EndScreen;
    [SerializeField]
    private SceneLoader _sceneLoader;
    [SerializeField]
    private SpawnController _enemySpawner;
    [SerializeField]
    private BezierSpline _MapPath;
    [SerializeField]
    private UnitPlacementHandler _placementHandler;
    [SerializeField]
    private UnitUpgrader _unitUpgrader;
    [SerializeField]
    private int _funds = 50;
    private int round = 0;

    private bool _isGameActive = true;

    public void Start() {
        Time.timeScale = 1;
        Instance = this;
        _isGameActive = true;
        UpdateHealth();
        UpdateUIRound();
        ModifyFunds(0);
    }

    /// <summary>
    /// get if the game is active;
    /// </summary>
    /// <returns></returns>
    public bool GetGameActive() {
        return _isGameActive;
    }

    /// <summary>
    /// Get the reference to the unit placing script
    /// </summary>
    /// <returns></returns>
    public UnitPlacementHandler GetPlacementHandler() {
        return _placementHandler;
    }

    /// <summary>
    /// Get the reference to the unit upgrader
    /// </summary>
    /// <returns></returns>
    public UnitUpgrader GetUpgrader() {
        return _unitUpgrader;
    }
    /// <summary>
    /// Update the health of the player
    /// </summary>
    /// <param name="damage"></param>
    public void UpdateHealth(int damage = 0) {
        _health -= damage;
        Mathf.Clamp(_health, 0, _health);
        _UIHealth.text = "HP: " + _health.ToString();

        if(_health <= 0) {
            GameEnd();
        }
    }

    /// <summary>
    /// increase or decrease the fund count
    /// </summary>
    /// <param name="incomingChange"></param>
    internal void ModifyFunds(int incomingChange) {
        _funds += incomingChange;
        _UIFunds.text = "Funds: \n" + _funds;
    }

    internal int GetFunds() {
        return _funds;
    }
    /// <summary>
    /// Update the UI text for the round count
    /// </summary>
    public void UpdateUIRound() {
        _UIRound.text =  "WAVE: " + round.ToString();
    }

    /// <summary>
    /// Start a round of the game
    /// </summary>
    public void StartRound() {
        round++;
        _enemySpawner.SpawnRound(round, 0.5f, _MapPath);
        UpdateUIRound();
    }

    /// <summary>
    /// The health is empty
    /// </summary>
    private void GameEnd() {
        _isGameActive = false;
        _EndScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResetLevel() {
        _sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
