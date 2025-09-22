using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image illustration;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button winButton;

    
    public void SetInfo(string title, Sprite illustrationSprite, string text)
    {
        titleText.text = title;
        illustration.sprite = illustrationSprite;
        //illustration.SetNativeSize();
        infoText.text = text;

        if (title == "GATO-TOSTADA")
        {
            winButton.gameObject.SetActive(true);
        }
        
    }

    public void OnQuitButton()
    {
        Destroy(gameObject);
    }

    public void OnWinButton()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
