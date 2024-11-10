using UnityEngine;
using TMPro;

public class LoadingScreenController : MonoBehaviour
{
  public GameObject loadingScreen;
  public TMP_Text loadingMessage;

  public void ShowLoadingScreen(string message)
  {
    loadingScreen.SetActive(true);
    loadingMessage.text = message;
  }

  public void HideLoadingScreen()
  {
    loadingScreen.SetActive(false);
  }
}
