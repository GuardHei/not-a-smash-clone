using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KO : MonoBehaviour {

    public Image img;

    [Range(.0f, 10.0f)]
    public float animDuration = 3.0f;

    private void Awake() {
        if (img == null) img = GetComponent<Image>();
        if (img != null) {
            img.gameObject.SetActive(false);
            img.type = Image.Type.Filled;
        }
    }

    public void Play() {
        img.gameObject.SetActive(true);
        img.fillAmount = .0f;
        StopAllCoroutines();
        StartCoroutine(AnimRoutine());
    }

    private IEnumerator AnimRoutine() {
        var startTime = Time.time;
        var currTime = startTime;
        var progress = currTime - startTime;
        while (progress < animDuration) {
            yield return null;
            img.fillAmount = progress / animDuration;
        }
        img.fillAmount = 1.0f;
    }
}
