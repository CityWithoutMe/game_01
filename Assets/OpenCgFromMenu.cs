using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class OpenCgFromMenu : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OpenCg);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OpenCg);
    }

    private void OpenCg()
    {
        SceneManager.LoadScene("cg");
    }
}
