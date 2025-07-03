using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private TileAnimator tileAnimator;
    private BuildManager buildManager;
    private Vector3 defaultPosition;
    private bool tileCanBeMovedByPlayer = true;
    private Coroutine currentMovementCoroutine; // Mengganti nama agar lebih jelas bahwa ini adalah coroutine

    private GameObject currentTowerInSlot = null;

    private void Awake()
    {
        // Pertimbangkan untuk meng-assign referensi ini melalui Inspector untuk performa yang lebih baik
        // dan menghindari ketergantungan pada nama atau keberadaan unik di scene.
        tileAnimator = FindFirstObjectByType<TileAnimator>();
        buildManager = FindFirstObjectByType<BuildManager>();
        
        defaultPosition = transform.position;

        if (tileAnimator == null) Debug.LogError($"TileAnimator tidak ditemukan pada BuildSlot: {gameObject.name}! Animasi tile mungkin tidak berfungsi.", this);
        if (buildManager == null) Debug.LogError($"BuildManager tidak ditemukan pada BuildSlot: {gameObject.name}! Fungsi build mungkin terganggu.", this);
    }

    // --- Metode untuk Interaksi Player (UI) ---
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (CanPlayerInteract() && currentTowerInSlot == null)
        {
            if (buildManager != null) buildManager.SelectBuildSlot(this);
            // Tidak perlu lagi memanggil MoveTileUp() secara langsung di sini jika BuildManager yang akan menanganinya
            // atau jika efek visual "terpilih" ditangani secara berbeda.
            // Jika efek visual "terangkat" saat diklik adalah keinginan:
            StartMoveTileUp(); 
            tileCanBeMovedByPlayer = false; // Tile terkunci setelah dipilih
        }
        // else if (currentTowerInSlot != null && buildManager != null)
        // {
        //    buildManager.SelectOccupiedSlot(this, currentTowerInSlot); // Untuk UI upgrade/sell
        // }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2)) // Abaikan jika tombol mouse lain ditekan
            return;

        if (!tileCanBeMovedByPlayer) // Jika tile terkunci (misalnya, sudah dipilih)
            return;
            
        if (!CanPlayerInteract() || currentTowerInSlot != null) // Jangan hover jika tidak bisa interaksi atau sudah ada tower
            return;

        StartMoveTileUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!tileCanBeMovedByPlayer) // Jika tile terkunci (misalnya, sudah dipilih dan sedang "up")
        {
            // Tile akan kembali ke posisi default hanya jika di-unselect secara eksplisit
            return;
        }

        // Jika tidak terkunci, tidak ada tower, dan bisa interaksi
        if (CanPlayerInteract() && currentTowerInSlot == null)
        {
            StartMoveToDefaultPosition();
        }
    }

    /// <summary>
    /// Dipanggil oleh BuildManager ketika slot ini tidak lagi dipilih oleh pemain.
    /// </summary>
    public void UnselectTileByPlayer()
    {
        tileCanBeMovedByPlayer = true; // Izinkan tile bergerak lagi saat hover
        StartMoveToDefaultPosition();
    }

    private void StartMoveTileUp()
    {
        if (tileAnimator == null) return;
        Vector3 targetPosition = defaultPosition + new Vector3(0, tileAnimator.GetBuildOffset(), 0); // Selalu dari defaultPosition + offset

        if (currentMovementCoroutine != null) StopCoroutine(currentMovementCoroutine);
        currentMovementCoroutine = StartCoroutine(RunMovementCoroutine(tileAnimator.MoveTileCo(transform, targetPosition)));
    }

    private void StartMoveToDefaultPosition()
    {
        if (tileAnimator == null) return;

        if (currentMovementCoroutine != null) StopCoroutine(currentMovementCoroutine);
        currentMovementCoroutine = StartCoroutine(RunMovementCoroutine(tileAnimator.MoveTileCo(transform, defaultPosition)));
    }

    /// <summary>
    /// Wrapper untuk menjalankan coroutine gerakan dan membersihkan flag setelah selesai.
    /// </summary>
    private IEnumerator RunMovementCoroutine(IEnumerator movementLogic)
    {
        if(movementLogic == null) {
            Debug.LogWarning("RunMovementCoroutine: movementLogic null.", this);
            currentMovementCoroutine = null;
            yield break;
        }
        yield return movementLogic; // Jalankan logika gerakan dari TileAnimator
        currentMovementCoroutine = null; // Bersihkan flag setelah selesai
    }

    // --- Metode untuk DDA & Pelatihan Agen (Programmatic Control) ---

    public bool CanBuildTowerProgrammatically()
    {
        return currentTowerInSlot == null;
    }

    public GameObject BuildTowerProgrammatically(GameObject towerPrefab)
    {
        if (CanBuildTowerProgrammatically() && towerPrefab != null)
        {
            Vector3 buildPosition = defaultPosition; // Tower selalu dibangun di posisi default tile
            currentTowerInSlot = Instantiate(towerPrefab, buildPosition, Quaternion.identity, transform);
            
            // Pastikan tile secara visual berada di posisi default jika dibangun secara programatik
            if (transform.position != defaultPosition)
            {
                if (currentMovementCoroutine != null) StopCoroutine(currentMovementCoroutine);
                transform.position = defaultPosition; // Langsung set
                currentMovementCoroutine = null;
            }
            tileCanBeMovedByPlayer = false; // Setelah ada tower, pemain tidak bisa hover-move tile lagi

            return currentTowerInSlot;
        }
        return null;
    }

    public void ClearTowerProgrammatically()
    {
        if (currentTowerInSlot != null)
        {
            Destroy(currentTowerInSlot);
            currentTowerInSlot = null;
        }
        tileCanBeMovedByPlayer = true; // Izinkan tile di-hover lagi
        
        // Pastikan tile kembali ke posisi default secara visual
        if (transform.position != defaultPosition)
        {
            StartMoveToDefaultPosition(); // Animasikan kembali ke default
        }
    }

    public GameObject GetCurrentTower()
    {
        return currentTowerInSlot;
    }

    private bool CanPlayerInteract()
    {
        // return buildManager != null && buildManager.IsPlayerInteractionAllowed(); // Implementasi yang lebih baik
        return true; // Implementasi sederhana
    }
}
