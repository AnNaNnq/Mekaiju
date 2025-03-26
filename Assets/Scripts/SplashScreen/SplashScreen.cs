using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float _waitTime = 5f;

    void Start()
    {
        StartCoroutine(WaitForEndOfSplashScreen());
    }

    private IEnumerator WaitForEndOfSplashScreen()
    {
        yield return new WaitForSeconds(_waitTime);

        SceneManager.LoadScene(1);
    }
}
