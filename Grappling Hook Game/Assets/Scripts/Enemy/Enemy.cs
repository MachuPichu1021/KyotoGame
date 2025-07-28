using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform projectileOutPoint;
    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private LayerMask groundLayer, playerLayer;

    [Header("Enemy Stats")]
    [SerializeField] private int hitpoints;

    [Header("Projectile Variables")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private float projectileCooldown;
    [SerializeField] private float attackRange;
    private bool playerInAttackRange;
    [SerializeField] private GameObject projectilePrefab;
    private float projectileTimer;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSFX;
    private AudioSource source;

    private void Start()
    {
        projectileTimer = projectileCooldown;

        source = GetComponent<AudioSource>();

        player = FindObjectOfType<PlayerMovement>().transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInAttackRange) agent.SetDestination(player.position);
        else
        {
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            projectileTimer -= Time.deltaTime;
            if (projectileTimer <= 0)
            {
                Shoot();
                projectileTimer = projectileCooldown;
            }
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

        GetHit();
    }

    private void GetHit()
    {
        hitpoints--;
        if (hitpoints <= 0) Destroy(gameObject);
    }
}
