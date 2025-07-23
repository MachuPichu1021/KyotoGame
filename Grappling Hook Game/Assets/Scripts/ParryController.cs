using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Camera cam;
    private float startFOV;

    [Header("Parry Variables")]
    [SerializeField] private float maxParryTime;
    private float parryActiveTimer;
    [SerializeField] private float parryMissCooldown;
    private float parryMissTimer;
    private bool hasParried = true;
    [SerializeField] private float hitstopLength;
    private bool isParrying;

    [Header("Parry Visuals")]
    [SerializeField] private Image parryImage;
    [SerializeField] private RectTransform parrySpriteTransform;
    [SerializeField] private Sprite parryReadySprite;
    [SerializeField] private Sprite parryActiveSprite;
    [SerializeField] private Sprite parryHit1Sprite;
    [SerializeField] private Sprite parryHit2Sprite;
    [SerializeField] private Sprite parryMissSprite;

    private readonly Vector2 parryPos1 = new Vector2(2120, -300);
    private readonly Vector2 parryPos2 = new Vector2(1945, -100);
    private readonly Vector2 parryPos3 = new Vector2(2600, -500);

    [Header("Audio")]
    [SerializeField] private AudioClip parryHitSFX;
    [SerializeField] private AudioClip parryCooldownSFX;

    [Header("Keybinds")]
    [SerializeField] private KeyCode parryKey;

    private void Start()
    {
        startFOV = cam.fieldOfView;
        parryImage.sprite = parryReadySprite;
        parrySpriteTransform.position = parryPos1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(parryKey) && parryMissTimer <= 0 && !isParrying) StartParry();
        else if (Input.GetKeyDown(parryKey) && parryMissTimer > 0)
        {
            AudioManager.instance.PlaySound(parryCooldownSFX);
            parryMissTimer += 0.25f;
        }
        if (isParrying) parryActiveTimer -= Time.deltaTime;
        if (parryActiveTimer <= 0 && isParrying) StopParry();

        if (parryMissTimer >= 0) parryMissTimer -= Time.deltaTime;
        else if (!isParrying)
        {
            parryImage.sprite = parryReadySprite;
            parrySpriteTransform.position = parryPos1;
        }
    }

    private void StartParry()
    {
        isParrying = true;
        parryActiveTimer = maxParryTime;
        parryImage.sprite = parryActiveSprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Projectile projectile)) return;

        if (isParrying)
        {
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projectile.lifetime = 10;
            float projSpeed = projRB.velocity.magnitude;

            projRB.velocity = projSpeed * 2 * cam.transform.forward.normalized;

            /*Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Array.Sort(enemies, (e1, e2) => Vector3.Distance(transform.position, e1.transform.position).CompareTo(Vector3.Distance(transform.position, e2.transform.position)));
            Enemy nearestEnemy = enemies[0];
            projRB.velocity = projSpeed * 2 * (nearestEnemy.transform.position - transform.position).normalized;*/

            parryActiveTimer += 0.1f;
            hasParried = true;
            AudioManager.instance.PlaySound(parryHitSFX);
            StartCoroutine(Hitstop());
            StartCoroutine(CameraZoom());
        }
        else { /* run a death method here */  print("Got Hit Stupid"); }
    }

    private void StopParry()
    {
        if (!hasParried)
        {
            parryMissTimer = parryMissCooldown;
            parryImage.sprite = parryMissSprite;
            parrySpriteTransform.position = parryPos3;
        }
        else parryImage.sprite = parryReadySprite;

        isParrying = false;
        hasParried = false;
    }

    private IEnumerator CameraZoom()
    {
        cam.fieldOfView = startFOV / 2;

        float t = 0;
        while (t < hitstopLength)
        {
            cam.fieldOfView = Mathf.Lerp(startFOV / 2, startFOV, t / hitstopLength);
            t += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = startFOV;
    }

    private IEnumerator Hitstop()
    {
        parryImage.sprite = parryHit1Sprite;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(hitstopLength);
        parryImage.sprite = parryHit2Sprite;
        parrySpriteTransform.position = parryPos2;
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(hitstopLength);
        parryImage.sprite = parryActiveSprite;
        parrySpriteTransform.position = parryPos1;
    }
}
