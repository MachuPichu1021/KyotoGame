using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Spawning Shit")]
    [SerializeField] private Transform[] enemySpawnpoints;
    private bool finishedSpawning = true;
    private int wave = 4;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject launcherPrefab;

    [Header("SFX")]
    [SerializeField] private AudioClip newWaveSFX;

    private void Update()
    {
        int enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        if (enemyCount <= 0 && finishedSpawning) OnEndWave();
    }

    private void OnEndWave()
    {
        wave++;
        finishedSpawning = false;
        AudioManager.instance.PlaySound(newWaveSFX);
        StartCoroutine(SpawnNewWave());
    }

    private IEnumerator SpawnNewWave()
    {
        int soldiersToSpawn = 2 * wave;
        int dronesToSpawn = 5 * (wave / 3);
        int launchersToSpawn = wave / 5;

        for (int i = 0; i < soldiersToSpawn; i++)
        {
            Vector3 spawnPos = enemySpawnpoints[Random.Range(0, enemySpawnpoints.Length)].position;
            Instantiate(soldierPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
        for (int i = 0; i < dronesToSpawn; i++)
        {
            Vector3 spawnPos = enemySpawnpoints[Random.Range(0, enemySpawnpoints.Length)].position;
            Instantiate(dronePrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
        for (int i = 0; i < launchersToSpawn; i++)
        {
            Vector3 spawnPos = enemySpawnpoints[Random.Range(0, enemySpawnpoints.Length)].position;
            Instantiate(launcherPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
        finishedSpawning = true;
    }
}
