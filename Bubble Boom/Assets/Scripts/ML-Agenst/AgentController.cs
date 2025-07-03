using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    public EnemyPortal portal;
    [SerializeField] private GridBuilder gridBuilder;
    [SerializeField] private List<GameObject> towerPrefabsList;

    private GameManager gameManagerInstance;

    public override void Initialize()
    {
        gameManagerInstance = GameManager.Instance;
    }
    public override void OnEpisodeBegin()
    {
        if (gameManagerInstance == null) gameManagerInstance = GameManager.Instance;

        portal.ClearAllEnemies();
        int randomTowerCount = Random.Range(1, 10);
        gameManagerInstance.AutoBuildTowersForTraining(gridBuilder, towerPrefabsList, randomTowerCount, true);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (gameManagerInstance == null) return;

        sensor.AddObservation(gameManagerInstance.GetTotalTowerPower());
        sensor.AddObservation(gameManagerInstance.GetTowerCount());
        sensor.AddObservation(gameManagerInstance.GetWaveIndex()); 
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Ambil aksi dari agent (jumlah & tipe musuh)
        int enemyCount = actions.DiscreteActions[0];     // 0–10
        int enemyType = actions.DiscreteActions[1];      // 0=normal, 1=fast, 2=tank, 3=healer (misal)
        float spawnRate = Mathf.Clamp(actions.ContinuousActions[0], 0.5f, 5f); // delay antar musuh

        // Panggil fungsi spawner
        if (portal.IsValidEnemyTypeIndex(enemyType))
        {
            portal.PrepareSpawn(enemyCount, enemyType, spawnRate);
        }
        else
        {
            Debug.LogWarning($"Agent memilih enemyTypeIndex tidak valid: {enemyType}");

            portal.PrepareSpawn(enemyCount, 0, spawnRate);
        }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Untuk testing manual
        var discreteActions = actionsOut.DiscreteActions;
        var continuousActions = actionsOut.ContinuousActions;
        
        if (Input.GetKey(KeyCode.Alpha1)) discreteActions[0] = 0; // Sedikit musuh
        if (Input.GetKey(KeyCode.Alpha2)) discreteActions[0] = 2; // Sedang
        if (Input.GetKey(KeyCode.Alpha3)) discreteActions[0] = 4; // Banyak

        // Misal untuk enemy type (branch 1): 0-2 (jika ada 3 tipe di enemyVariants)
        if (Input.GetKey(KeyCode.Q)) discreteActions[1] = 0; // Tipe Normal
        if (Input.GetKey(KeyCode.W)) discreteActions[1] = 1; // Tipe Fast
        if (Input.GetKey(KeyCode.E)) discreteActions[1] = 2; // Tipe Tank

        // Misal untuk spawn rate (continuous branch 0):
        if (Input.GetKey(KeyCode.PageUp)) continuousActions[0] = 0.5f;   // Cepat
        if (Input.GetKey(KeyCode.PageDown)) continuousActions[0] = 3.0f; // Lambat
    }

    public void AssignRewardAndEndEpisode(float playerBaseHpPercentage) // Ubah nama parameter agar lebih jelas
    {
        // Reward berdasarkan persentase HP basis pemain di akhir wave
        // Persentase HP lebih baik daripada HP absolut untuk normalisasi
        if (playerBaseHpPercentage <= 0.01f) // Kalah telak (gunakan epsilon kecil untuk float comparison)
        {
            SetReward(-1.0f); // Hukuman besar
            Debug.Log($"Episode End: Kalah. Reward: -1.0f. Player HP: {playerBaseHpPercentage * 100}%");
        }
        else if (playerBaseHpPercentage >= 0.95f) // Terlalu mudah (misal, HP hampir penuh)
        {
            SetReward(-0.5f); // Hukuman sedang, agen membuat wave terlalu gampang
            Debug.Log($"Episode End: Terlalu Mudah. Reward: -0.5f. Player HP: {playerBaseHpPercentage * 100}%");
        }
        else if (playerBaseHpPercentage >= 0.4f && playerBaseHpPercentage <= 0.7f) // Target ideal (misal, HP 40%-70%)
        {
            SetReward(1.0f);  // Reward tertinggi, tantangan seimbang
             Debug.Log($"Episode End: Seimbang. Reward: 1.0f. Player HP: {playerBaseHpPercentage * 100}%");
        }
        else if (playerBaseHpPercentage > 0.7f && playerBaseHpPercentage < 0.95f) // Agak terlalu mudah
        {
            SetReward(0.2f); // Reward kecil positif
            Debug.Log($"Episode End: Agak Mudah. Reward: 0.2f. Player HP: {playerBaseHpPercentage * 100}%");
        }
        else if (playerBaseHpPercentage > 0.01f && playerBaseHpPercentage < 0.4f) // Agak terlalu sulit tapi tidak kalah
        {
            SetReward(0.5f); // Reward positif, tapi bisa lebih baik
             Debug.Log($"Episode End: Agak Sulit. Reward: 0.5f. Player HP: {playerBaseHpPercentage * 100}%");
        }
        else
        {
             SetReward(0f); // Kondisi lain (netral)
             Debug.Log($"Episode End: Netral. Reward: 0f. Player HP: {playerBaseHpPercentage * 100}%");
        }


        EndEpisode(); // Akhiri episode agar agen bisa memulai yang baru dengan observasi baru
    }

}
