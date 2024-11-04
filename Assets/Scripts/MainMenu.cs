using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject controlsPanel;
    public TMP_Dropdown winThresholdDropdown; 

    private void Start()
    {
        int defaultThreshold = 3;
        int savedThreshold = PlayerPrefs.GetInt("WinThreshold", defaultThreshold);

        //set dropdown to saved option
        int dropdownIndex = winThresholdDropdown.options.FindIndex(option => option.text == savedThreshold.ToString());
        winThresholdDropdown.value = dropdownIndex >= 0 ? dropdownIndex : 0;
    }

    public void SetWinThreshold()
    {
        //handles dropdown menu win threshold
        int threshold = int.Parse(winThresholdDropdown.options[winThresholdDropdown.value].text);
        PlayerPrefs.SetInt("WinThreshold", threshold);
        PlayerPrefs.Save();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ShowControls()
    {
        controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
    }
}




