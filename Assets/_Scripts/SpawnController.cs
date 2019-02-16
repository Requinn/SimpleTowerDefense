using System.Collections;
using UnityEngine;
using JLProject.Spline;

public class SpawnController : MonoBehaviour
{
    public Enemy _enemyPrefab;
    public Transform unitSpawnLocation;
    int deathCount = 0;
    int deathNeeeded = 0;

    public delegate void RoundEndedEvent();
    public RoundEndedEvent RoundEnd;

    private float _currentScaleHealth = 1f, _currentScaleSpeed = 1f, _currentScaleDamage = 1f, _currentScaleFunds = 1f;
    /// <summary>
    /// Run the coroutine to spawn the wave
    /// </summary>
    /// <param name="round">used to adjust scaling</param>
    /// <param name="count">How many to spawn</param>
    /// <param name="spawnDelay">How much time in between spawns</param>
    /// <param name="path">Which path to take</param>
    public void SpawnRound(int round, float spawnDelay, BezierSpline path) {
        //every 5 rounds above 0
        if(round > 0 && round % 5 == 0) {
            _currentScaleHealth += 1f;
            _currentScaleDamage += 0.5f;
            _currentScaleFunds += 0.05f;
            _currentScaleSpeed += 0.1f;
        }
        StartCoroutine(Spawn(Mathf.CeilToInt(5 * Mathf.Sqrt(round)), spawnDelay, path));
    }

    /// <summary>
    /// Spawn a wave of enemies
    /// </summary>
    /// <param name="count"></param>
    /// <param name="delay"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private IEnumerator Spawn(int count, float delay, BezierSpline path) {
        deathNeeeded = count;
        deathCount = 0;
        //while things are left to spawn
        while (count > 0) {
            //create
            Enemy  e = Instantiate(_enemyPrefab, unitSpawnLocation.position, Quaternion.identity);
            //asign apth
            e.Initialize(path, _currentScaleDamage, _currentScaleHealth, _currentScaleSpeed, _currentScaleFunds);
            //sub to death event and end reached event
            e.DeathEvent += CountDeaths;
            e.EndReached += GameController.Instance.UpdateHealth;
            count--;
            //wait delay
            yield return new WaitForSeconds(delay);
        }
        yield return 0f;
    }

    private void CountDeaths(int unitValue) {
        deathCount++;
        GameController.Instance.ModifyFunds(unitValue);
        if(deathCount >= deathNeeeded) {
            if(RoundEnd != null) RoundEnd();
        }
    }
}
