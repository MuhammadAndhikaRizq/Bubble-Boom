using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleBullet : MonoBehaviour
{
    private BubbleGun myTower;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackDuration = .1f;
    [SerializeField] private float bulletDuration = 0.5f;

    private void Awake()
    {
        myTower = GetComponent<BubbleGun>();
    }

    public void PlayAttack(Vector3 startPoin, Vector3 endPoin)
    {
        StartCoroutine(BulletCoroutine(startPoin, endPoin));
    }

    private IEnumerator BulletCoroutine(Vector3 startPoin, Vector3 endPoin)
    {
        myTower.EnableRotation(false); // Nonaktifkan rotasi tower saat menembak

        // Instantiate bullet effect
        GameObject bullet = Instantiate(bulletPrefab, startPoin, Quaternion.identity);
        
        // Atur rotasi bullet agar mengarah ke target
        bullet.transform.LookAt(endPoin);
        bullet.transform.position = Vector3.Lerp(startPoin, endPoin, attackDuration * Time.deltaTime);
            yield return new WaitForSeconds(attackDuration);

        bullet.transform.position = endPoin; // Pastikan peluru mencapai tujuan
        Destroy(bullet, 0.1f);

        myTower.EnableRotation(true); // Aktifkan kembali rotasi tower
    }
}
