using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoor : MonoBehaviour
{
    private bool opened;
    private void Update()
    {
        Enemy[] enemiesFound = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        if (enemiesFound.Length == 0 && !opened) StartCoroutine(Open());
    }

    private IEnumerator Open()
    {
        float startHeight = transform.position.y;
        float endHeight = transform.position.y + transform.localScale.z / 2;
        float t = 0;
        opened = true;

        while (t < 5)
        {
            float yPos = Mathf.Lerp(startHeight, endHeight, t / 5);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, endHeight, transform.position.z);
    }
}
