using System.Collections;
using UnityEngine;

public class VisualBullet : MonoBehaviour
{
    private BubbleGun myTower;
    [SerializeField] private GameObject bulletPrefab;
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
        myTower.EnableRotation(false);

        // Instantiate bullet effect
        GameObject bullet = Instantiate(bulletPrefab, startPoin, Quaternion.identity);
        ParticleSystem bulletParticle = bullet.GetComponent<ParticleSystem>();

        if (bulletParticle != null)
        {
            bulletParticle.Play();
        }

        // Move bullet from startPoin to endPoin
        float elapsedTime = 0;
        while (elapsedTime < bulletDuration)
        {
            bullet.transform.position = Vector3.Lerp(startPoin, endPoin, elapsedTime / bulletDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop particle and destroy bullet
        if (bulletParticle != null)
        {
            bulletParticle.Stop();
        }
        Destroy(bullet, 0.5f); // Delay sedikit sebelum dihancurkan

        myTower.EnableRotation(true);
    }
}
