using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform projectileOutPoint;

    [Header("Projectile Variables")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private float projectileCooldown;
    [SerializeField] private GameObject projectilePrefab;
    private float projectileTimer;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSFX;
    private AudioSource source;

    private void Start()
    {
        projectileTimer = projectileCooldown;

        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        projectileTimer -= Time.deltaTime;
        if (projectileTimer <= 0)
        {
            Shoot();
            projectileTimer = projectileCooldown;
        }
    }

    private void Shoot()
    {
        Projectile projectile = Instantiate(projectilePrefab, projectileOutPoint.position, projectileOutPoint.rotation).GetComponent<Projectile>();
        projectile.lifetime = projectileLifeTime;
        projectile.GetComponent<Rigidbody>().velocity = projectileSpeed * projectileOutPoint.forward;
        source.PlayOneShot(shootSFX);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Projectile _)) return;

        Destroy(gameObject);
        //print("Enemy Got Hit");
    }
}
