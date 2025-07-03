using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    // Variabel untuk menyimpan BuildSlot yang sedang dipilih saat ini
    public BuildSlot selectedBuildSlot { get; private set; } // Menggunakan property dengan private set untuk kontrol lebih

    /// <summary>
    /// Dipanggil ketika pemain memilih sebuah BuildSlot.
    /// </summary>
    /// <param name="newSlot">BuildSlot baru yang dipilih.</param>
    public void SelectBuildSlot(BuildSlot newSlot)
    {
        // Jika ada slot yang sudah dipilih sebelumnya dan itu bukan slot yang sama dengan yang baru dipilih
        if (selectedBuildSlot != null && selectedBuildSlot != newSlot)
        {
            // Panggil metode untuk 'membatalkan pilihan' pada slot lama
            selectedBuildSlot.UnselectTileByPlayer(); 
        }
        
        // Tetapkan slot baru sebagai slot yang dipilih
        selectedBuildSlot = newSlot;

        // Di sini Anda bisa menambahkan logika lain setelah slot dipilih,
        // misalnya, menampilkan UI untuk membangun tower jika newSlot tidak null dan kosong.
        if (selectedBuildSlot != null)
        {
            Debug.Log($"BuildSlot dipilih: {selectedBuildSlot.name}");
            // Contoh: TowerUIManager.Instance.ShowBuildOptionsForSlot(selectedBuildSlot);
        }
    }

    /// <summary>
    /// Dipanggil untuk membatalkan pilihan slot saat ini (misalnya, jika pemain menekan tombol Escape atau memilih untuk tidak membangun).
    /// </summary>
    public void DeselectCurrentSlot()
    {
        if (selectedBuildSlot != null)
        {
            selectedBuildSlot.UnselectTileByPlayer();
            selectedBuildSlot = null;
            Debug.Log("BuildSlot saat ini dibatalkan pilihannya.");
            // Contoh: TowerUIManager.Instance.HideBuildOptions();
        }
    }

    // Anda bisa menambahkan metode lain di sini, misalnya untuk membangun tower:
    // public bool TryBuildTowerOnSelectedSlot(GameObject towerPrefab)
    // {
    //     if (selectedBuildSlot != null && selectedBuildSlot.GetCurrentTower() == null)
    //     {
    //         GameObject builtTower = selectedBuildSlot.BuildTowerProgrammatically(towerPrefab);
    //         if (builtTower != null)
    //         {
    //             Debug.Log($"Tower {towerPrefab.name} berhasil dibangun di {selectedBuildSlot.name}");
    //             DeselectCurrentSlot(); // Batalkan pilihan slot setelah membangun
    //             return true;
    //         }
    //     }
    //     Debug.LogWarning("Tidak bisa membangun tower. Slot tidak dipilih atau sudah terisi.");
    //     return false;
    // }
}
