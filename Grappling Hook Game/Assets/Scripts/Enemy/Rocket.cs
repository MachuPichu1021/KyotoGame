using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private bool targetPlayer;
    private Rigidbody rb;

    [SerializeField] private AudioClip rocketFlySFX;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        AudioSource source = GetComponent<AudioSource>();
        source.clip = rocketFlySFX;
        source.Play();
        StartTargettingPlayer();
    }

    private void Update()
    {
        if (targetPlayer)
        {
            Transform player = FindObjectOfType<PlayerMovement>().transform;
            transform.LookAt(player);
            rb.velocity = transform.forward * rb.velocity.magnitude;
        }
    }

    private void StartTargettingPlayer()
    {
        targetPlayer = true;
    }
}
