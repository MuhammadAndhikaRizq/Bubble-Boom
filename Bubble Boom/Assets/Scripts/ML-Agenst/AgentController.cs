using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEditor.Build.Content;

public class AgentController : Agent
{
    public EnemyPortal portal;
    [SerializeField] private GridBuilder gridBuilder;
    [SerializeField] private List<GameObject> towerPrefabsList;

    public override void OnEpisodeBegin()
    {
        portal.ClearAllEnemies();
        GameManager.Instance.AutoBuildTowersForTraining(gridBuilder, towerPrefabsList, 3);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(GameManager.Instance.TotalTowerPower());
        sensor.AddObservation(GameManager.Instance.GetWaveIndex());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Ambil aksi dari agent (jumlah & tipe musuh)
        int enemyCount = actions.DiscreteActions[0];     // 0–10
        int enemyType = actions.DiscreteActions[1];      // 0=normal, 1=fast, 2=tank, 3=healer (misal)
        float spawnRate = Mathf.Clamp(actions.ContinuousActions[0], 0.5f, 5f); // delay antar musuh

        // Panggil fungsi spawner
        portal.PrepareSpawn(enemyCount, enemyType, spawnRate);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Untuk testing manual
        var d = actionsOut.DiscreteActions;
        d[0] = 5;
        d[1] = 0;
        var c = actionsOut.ContinuousActions;
        c[0] = 2f;
    }

    public void RewardEpisode(float hpResult)
    {
        // Reward berdasarkan hasil wave
        if (hpResult <= 0)
            SetReward(-1f); // Pemain kalah
        else if (hpResult >= 0.9f)
            SetReward(-0.5f); // Terlalu mudah
        else if (hpResult >= 0.5f)
            SetReward(1f); // Seimbang
        else
            SetReward(0.5f); // Sedikit sulit

        EndEpisode();
    }

}
