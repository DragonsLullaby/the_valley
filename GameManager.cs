using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject pauseScreen;
    public GameObject inventoryScreen;
    public GameObject deathScreen;
    public GameObject blackFade;
    public GameObject craftingObj;
    public GameObject cookingObj;
    public GameObject craftingTab;
    public GameObject cookingTab;

    public GameObject inventoryBox;
    public GameObject craftingBox;
    public GameObject radioBox;

    public GameObject endGameScreen;

    Vector3 defaultPos;
    public bool gamePaused;
    public bool inventoryOpen;
    public bool craftingOpen;
    public bool radioOpen;
    bool playerDead;
    bool isFade;

    void Start()
    {
        defaultPos = player.transform.position;
        gamePaused = false;
        inventoryOpen = false;
        craftingOpen = false;
        radioOpen = false;
        playerDead = false;
        isFade = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ----- check if player presses Escape or Tab -----
        if (Input.GetKeyDown(KeyCode.Escape) && !gamePaused && !inventoryOpen)
            PauseGame();
        else if (Input.GetKeyDown(KeyCode.Escape) && gamePaused && !playerDead && !isFade)
            UnpauseGame();
        else if (Input.GetKeyDown(KeyCode.Tab) && !inventoryOpen && !gamePaused)
            OpenInventory();
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && inventoryOpen && !playerDead && !craftingOpen && !radioOpen)
            CloseInventory();
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && craftingOpen && !playerDead)
            CloseCrafting();
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && radioOpen && !playerDead)
            CloseRadio();

        // ----- check if player falls off the map -----
        if (player.transform.position.y <= -5)
            player.transform.position = defaultPos;
    }

    void PauseGame()
    {
        gamePaused = true;
        pauseScreen.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnpauseGame()
    {
        pauseScreen.SetActive(false);
        gamePaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OpenInventory()
    {
        inventoryOpen = true;
        inventoryBox.SetActive(true);
        craftingBox.SetActive(false);
        radioBox.SetActive(false);
        inventoryScreen.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void CloseInventory()
    {
        inventoryScreen.SetActive(false);
        inventoryOpen = false;
        GetComponent<InventorySystem>().ClearPreviewSlot("inventory");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenCrafting()
    {
        inventoryOpen = true;
        craftingOpen = true;
        inventoryBox.SetActive(false);
        radioBox.SetActive(false);
        craftingBox.SetActive(true);
        craftingObj.SetActive(true);
        cookingObj.SetActive(false);
        craftingTab.GetComponent<Button>().interactable = false;
        cookingTab.GetComponent<Button>().interactable = true;

        GetComponent<InventorySystem>().craftingTab = true;
        inventoryScreen.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseCrafting()
    {
        inventoryOpen = false;
        craftingOpen = false;
        inventoryScreen.SetActive(false);
        GetComponent<InventorySystem>().ClearPreviewSlot("crafting");
        GetComponent<InventorySystem>().ClearPreviewSlot("cooking");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenRadio()
    {
        inventoryOpen = true;
        radioOpen = true;

        inventoryBox.SetActive(false);
        craftingBox.SetActive(false);
        radioBox.SetActive(true);

        GetComponent<InventorySystem>().radioScreen = true;
        inventoryScreen.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseRadio()
    {
        inventoryOpen = false;
        radioOpen = false;

        radioBox.SetActive(false);
        inventoryScreen.SetActive(false);
        GetComponent<InventorySystem>().radioScreen = false;
        GetComponent<InventorySystem>().ClearPreviewSlot("radio");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        //player died and restarted the game
        SceneManager.LoadScene("SampleScene");
    }

    public void PlayerDied(string text)
    {
        //the player dies and loses the game
        playerDead = true;
        gamePaused = true;

        if (inventoryScreen.activeSelf)
            inventoryScreen.SetActive(false);

        deathScreen.transform.Find("DeathMessage").gameObject.GetComponent<TextMeshProUGUI>().text = text;
        deathScreen.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartSleep()
    {
        StartCoroutine(FadeBlack());
        gamePaused = true;
        isFade = true;
    }

    public IEnumerator FadeBlack(bool fadeToBlack = true, int fadeSpeed = 1, bool sleep = true)
    {
        Color objectColor = blackFade.GetComponent<Image>().color;
        float fadeAmount;

        if (fadeToBlack)
        {
            while (blackFade.GetComponent<Image>().color.a < 1)
            {
                // starts at 0 and increases to 1
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackFade.GetComponent<Image>().color = objectColor;
                yield return null;

                if (blackFade.GetComponent<Image>().color.a >= 1 && sleep)
                {
                    // fade is done
                    PlayerSleeps();
                }
                else if (blackFade.GetComponent<Image>().color.a >= 1 && !sleep)
                {
                    // fade is done
                    // activate end game screen
                    endGameScreen.SetActive(true);
                }
            }
        }
        else
        {
            while (blackFade.GetComponent<Image>().color.a > 0)
            {
                // starts at 1 and decreases to 0
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackFade.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
    }

    void PlayerSleeps()
    {
        // initial fade to black is done
        // set time to morning
        GameObject.Find("---Sky---").GetComponent<DayCycle>().Sleep();
    }

    public void EndSleep()
    {
        // need to wait for fade in to finish completely
        StartCoroutine(Wait());
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeBlack(false));
        isFade = false;
        gamePaused = false;
    }

    public void EndGame()
    {
        gamePaused = true;
        isFade = true;
        StartCoroutine(FadeBlack(true, 1, false));
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
