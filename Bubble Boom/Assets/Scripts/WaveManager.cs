using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.XR; // Kemungkinan tidak diperlukan

[System.Serializable]
public class WaveDetails // Digunakan jika Anda ingin progresi layout atau event khusus antar wave DDA
{
    [Tooltip("Prefab atau referensi ke GridBuilder di scene yang mendefinisikan layout berikutnya.")]
    public GridBuilder nextGridTemplate; // Mengganti nama agar lebih jelas bahwa ini adalah template
    [Tooltip("Portal baru yang akan diaktifkan untuk layout ini (jika ada).")]
    public EnemyPortal[] additionalPortalsToActivate; // Mengganti nama
    // Anda bisa menambahkan detail lain yang spesifik untuk perubahan layout ini
    // public int basicEnemy; // Jumlah musuh dan tipe akan ditentukan oleh Agen DDA
    // public int fastEnemy;
}

public class WaveManager : MonoBehaviour
{
    [Header("Referensi Inti")]
    [SerializeField] private GridBuilder currentActiveGrid; // Mengganti nama dan pastikan ini di-assign
    [SerializeField] private AgentController agentController; // Mengganti nama agar konsisten

    [Header("Pengaturan Wave")]
    public bool waveIsInProgress; // Mengganti nama agar lebih jelas
    public float timeBetweenWaves = 5f;
    private float interWaveTimer;

    [Header("Progresi Layout (Opsional untuk DDA)")]
    [Tooltip("Array WaveDetails untuk mengubah layout atau mengaktifkan portal antar episode DDA.")]
    [SerializeField] private WaveDetails[] scriptedLayoutProgression;
    private int currentLayoutProgressionIndex;

    [Header("Pengaturan Pengecekan")]
    private float checkWaveEndInterval = 0.5f;
    private float nextWaveEndCheckTime;

    // Daftar portal yang saat ini aktif dan dikelola oleh WaveManager atau Agen
    // Agen mungkin hanya mengontrol satu portal, atau WaveManager bisa mengelola beberapa
    public List<EnemyPortal> activeEnemyPortals;
    [SerializeField] private GameManager gameManager; // Cache instance GameManager

    private void Awake()
    {
        gameManager = GameManager.Instance; // Cache GameManager
        if (gameManager == null) Debug.LogError("GameManager instance tidak ditemukan!", this);

        if (agentController == null) Debug.LogError("AgentController tidak di-assign di WaveManager!", this);
        if (currentActiveGrid == null) Debug.LogError("Current Active Grid tidak di-assign di WaveManager!", this);

        // Inisialisasi daftar portal aktif
        // Bisa diisi dari scene atau portal yang dikontrol agen secara spesifik
        activeEnemyPortals = new List<EnemyPortal>(FindObjectsOfType<EnemyPortal>());
        if (activeEnemyPortals.Count == 0 && agentController != null && agentController.portal != null)
        {
            // Jika agen mengontrol satu portal spesifik dan tidak ada portal lain ditemukan
            activeEnemyPortals.Add(agentController.portal);
        }
        else if (activeEnemyPortals.Count == 0)
        {
            Debug.LogWarning("Tidak ada EnemyPortal aktif yang ditemukan di scene atau di-assign ke agen.", this);
        }
    }

    private void Start()
    {
        // Episode pertama agen akan dipanggil dari Agent.OnEpisodeBegin(),
        // yang biasanya berjalan sebelum Start() untuk GameObject yang aktif.
        // Jadi, wave pertama dianggap langsung berjalan.
        waveIsInProgress = true;
        interWaveTimer = 0;
        currentLayoutProgressionIndex = 0; // Mulai dari layout progression pertama (jika ada)

        // Terapkan layout awal jika ada di scriptedLayoutProgression[0]
        // Ini berguna jika Anda ingin wave pertama DDA dimulai dengan layout tertentu.
        if (scriptedLayoutProgression != null && scriptedLayoutProgression.Length > 0)
        {
            ApplyLayoutChanges(scriptedLayoutProgression[0]);
        }
    }

    private void Update()
    {
        if (waveIsInProgress)
        {
            CheckForWaveCompletion();
        }
        else
        {
            ManageInterWaveTimer();
        }
    }

    private void CheckForWaveCompletion()
    {
        if (!IsReadyForNextCheck()) return;

        if (AreAllEnemiesDefeated() && HaveAllPortalsFinishedSpawning())
        {
            waveIsInProgress = false;
            interWaveTimer = timeBetweenWaves;

            float playerHpPercentage = gameManager.GetPlayerBaseHealthPercentage();
            agentController.AssignRewardAndEndEpisode(playerHpPercentage); // Ini akan memicu OnEpisodeBegin untuk wave berikutnya

            Debug.Log($"Wave DDA berakhir. Player HP: {playerHpPercentage * 100}%. Memulai timer antar wave ({timeBetweenWaves}s).");

            // Tingkatkan Wave Index Global yang mungkin diobservasi Agen
            gameManager.IncrementWaveIndex();

            // Terapkan perubahan layout berikutnya dari progresi yang sudah diskrip (jika ada)
            // Ini akan diterapkan *sebelum* agen memulai episode barunya (OnEpisodeBegin)
            currentLayoutProgressionIndex++; // Pindah ke detail layout berikutnya
            if (scriptedLayoutProgression != null && currentLayoutProgressionIndex < scriptedLayoutProgression.Length)
            {
                ApplyLayoutChanges(scriptedLayoutProgression[currentLayoutProgressionIndex]);
            }
            else if (scriptedLayoutProgression != null && currentLayoutProgressionIndex >= scriptedLayoutProgression.Length)
            {
                Debug.Log("Semua progresi layout yang diskrip telah selesai.");
                // Anda bisa memutuskan untuk mengulang layout atau berhenti mengubahnya
            }
        }
    }

    private void ManageInterWaveTimer()
    {
        interWaveTimer -= Time.deltaTime;
        if (interWaveTimer <= 0)
        {
            // Waktu antar wave selesai. Agen sudah memulai episode baru (OnEpisodeBegin dipicu oleh EndEpisode).
            waveIsInProgress = true;
            Debug.Log("Timer antar wave selesai. Wave DDA baru (episode agen) aktif.");
        }
    }

    /// <summary>
    /// Menerapkan perubahan layout berdasarkan WaveDetails.
    /// </summary>
    private void ApplyLayoutChanges(WaveDetails layoutDetails)
    {
        if (layoutDetails == null) return;

        Debug.Log($"Menerapkan perubahan layout dari progresi ke-{currentLayoutProgressionIndex}.");

        if (layoutDetails.nextGridTemplate != null && currentActiveGrid != null)
        {
            UpdateActiveGridTiles(layoutDetails.nextGridTemplate);
        }

        if (layoutDetails.additionalPortalsToActivate != null && layoutDetails.additionalPortalsToActivate.Length > 0)
        {
            ActivateAndRegisterNewPortals(layoutDetails.additionalPortalsToActivate);
        }

        if (currentActiveGrid != null) currentActiveGrid.UpdateNavMesh();
    }

    /// <summary>
    /// Mengupdate tile pada grid aktif berdasarkan template grid baru.
    /// </summary>
    private void UpdateActiveGridTiles(GridBuilder newGridLayoutTemplate)
    {
        if (currentActiveGrid == null || newGridLayoutTemplate == null)
        {
            Debug.LogWarning("UpdateActiveGridTiles dibatalkan: currentActiveGrid atau newGridLayoutTemplate null.");
            return;
        }

        // Dapatkan daftar INSTANCE tile saat ini dari grid aktif
        List<GameObject> currentTileInstances = currentActiveGrid.GetTileGameObjectsSetup();
        // Dapatkan daftar (seharusnya) PREFAB atau definisi tile dari template grid baru
        // Asumsi: GetTileGameObjectsSetup() pada template mengembalikan definisi tile (misal, list prefab per posisi)
        // atau instance tile dari scene template yang akan kita tiru.
        // Untuk skrip GridBuilder di Canvas, GetTileGameObjectsSetup() mengembalikan INSTANCE.
        // Jadi, kita akan meng-clone properti atau meng-instantiate ulang berdasarkan prefab dari instance template.
        List<GameObject> newTileDefinitions = newGridLayoutTemplate.GetTileGameObjectsSetup();

        if (currentTileInstances.Count != newTileDefinitions.Count)
        {
            Debug.LogError("Jumlah tile di grid aktif dan template grid baru tidak sama! Tidak bisa update layout.");
            return;
        }

        // List untuk menyimpan instance tile baru yang akan menggantikan yang lama di currentActiveGrid
        List<GameObject> newTileInstancesForCurrentGrid = new List<GameObject>(currentTileInstances.Count);

        for (int i = 0; i < currentTileInstances.Count; i++)
        {
            GameObject oldTileInstance = currentTileInstances[i];
            GameObject newTileDefinition = newTileDefinitions[i]; // Ini adalah instance dari template grid

            if (oldTileInstance != null && newTileDefinition != null)
            {
                Vector3 position = oldTileInstance.transform.position; // Pertahankan posisi tile lama
                Transform parent = currentActiveGrid.transform;

                // Dapatkan prefab asli dari newTileDefinition (jika itu instance)
                // Ini agak rumit jika newTileDefinition adalah instance. Cara paling mudah adalah
                // jika newTileDefinition memiliki referensi ke prefab aslinya.
                // Untuk kesederhanaan, kita asumsikan newTileDefinition *adalah* prefab yang diinginkan
                // atau kita meng-instantiate salinannya.
                // Jika newGridLayoutTemplate adalah prefab, maka newTileDefinition adalah bagian dari prefab itu.
                // Jika newGridLayoutTemplate adalah instance di scene, maka newTileDefinition adalah instance.

                // Cara aman: Instantiate newTileDefinition sebagai tile baru.
                // Ini mengasumsikan newTileDefinition adalah prefab atau instance yang valid untuk di-clone.
                GameObject newTileInstance = Instantiate(newTileDefinition, position, newTileDefinition.transform.rotation, parent);
                newTileInstance.name = oldTileInstance.name; // Pertahankan nama lama atau beri nama baru

                // Hancurkan tile lama
                Destroy(oldTileInstance);

                newTileInstancesForCurrentGrid.Add(newTileInstance);
            }
            else
            {
                // Jika salah satu null, mungkin tambahkan tile lama (jika ada) atau null
                newTileInstancesForCurrentGrid.Add(oldTileInstance);
                 Debug.LogWarning($"Tile lama atau definisi tile baru null pada index {i} saat update layout.");
            }
        }

        // Update list internal di currentActiveGrid.
        // Ini memerlukan GridBuilder memiliki metode untuk mengganti list tile-nya.
        // Berdasarkan Canvas GridBuilder: `GetTileGameObjectsSetup()` mengembalikan referensi langsung,
        // jadi kita bisa Clear dan AddRange, atau buat metode Set di GridBuilder.
        // Untuk saat ini, kita asumsikan modifikasi langsung (meski kurang ideal dari segi enkapsulasi).
        currentTileInstances.Clear();
        currentTileInstances.AddRange(newTileInstancesForCurrentGrid);
        // Jika GridBuilder.createdTileGameObjects adalah yang dikembalikan oleh GetTileGameObjectsSetup(),
        // maka baris di atas sudah memodifikasinya.

        Debug.Log("Layout grid aktif telah diupdate.");
    }


    private void ActivateAndRegisterNewPortals(EnemyPortal[] portalsToActivate)
    {
        foreach (EnemyPortal portal in portalsToActivate)
        {
            if (portal != null)
            {
                portal.gameObject.SetActive(true);
                if (!activeEnemyPortals.Contains(portal))
                {
                    activeEnemyPortals.Add(portal);
                     Debug.Log($"Portal {portal.name} diaktifkan dan ditambahkan ke daftar aktif.");
                }
            }
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        foreach (EnemyPortal portal in activeEnemyPortals)
        {
            if (portal != null && portal.GetActiveEnemies().Count > 0)
                return false;
        }
        return true;
    }

    private bool HaveAllPortalsFinishedSpawning()
    {
        foreach (EnemyPortal portal in activeEnemyPortals)
        {
            if (portal != null && !portal.AreAllEnemiesInQueueSpawned()) // Asumsi EnemyPortal punya method ini
                return false;
        }
        return true;
    }

    private bool IsReadyForNextCheck()
    {
        if (Time.time >= nextWaveEndCheckTime)
        {
            nextWaveEndCheckTime = Time.time + checkWaveEndInterval;
            return true;
        }
        return false;
    }
}
