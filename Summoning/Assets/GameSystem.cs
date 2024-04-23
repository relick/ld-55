using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public Player player;
    public GameObject canvas;
    public TextMeshProUGUI leftPage;
    public TextMeshProUGUI rightPage;
    public Sprite coloredImage;

    public FenceAdder fenceCentre;

    public Image lightness;
    public float fadingIn = 1.0f;

    public InputActionAsset actions;

    private InputAction nextPageAction;
    private InputAction prevPageAction;
    private InputAction exitAction;
    void Start()
    {
        canvas.SetActive(false);
        lightness.CrossFadeAlpha(0.0f, fadingIn, false);
        darkness.CrossFadeAlpha(0.0f, 0.0f, false);
        colourPropID = Shader.PropertyToID("_Level");

        nextPageAction = actions.FindActionMap("reading").FindAction("NextPage");
        nextPageAction.performed += TurnRight;
        prevPageAction = actions.FindActionMap("reading").FindAction("PrevPage");
        prevPageAction.performed += TurnLeft;
        exitAction = actions.FindActionMap("reading").FindAction("Exit");
        exitAction.performed += CloseBook;
    }

    private int colourPropID = -1;
    public Material colourWorld;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, fenceCentre.transform.position) > fenceCentre.radius && booksRead.Count < 3)
        {
            player.Pause();
            Kill();
        }

        if(killing)
        {
            if(killingFadeout + 0.2f < killingFadeoutTimer)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
            killingFadeoutTimer += Time.deltaTime;
        }

        if(restoringColour)
        {
            restoringColourTimer -= Time.deltaTime;
            if(restoringColourTimer < 0)
            {
                restoringColourTimer = 0;
                restoringColour = false;
            }

            colourWorld.SetFloat(colourPropID, 1.0f - (restoringColourTimer / restoringColourSpeed));
        }
    }

    private bool killing = false;
    public float killingFadeout = 1.5f;
    private float killingFadeoutTimer = 0.0f;
    public Image darkness;
    void Kill()
    {
        killing = true;
        darkness.CrossFadeAlpha(1.0f, killingFadeout, false);
    }

    private List<Interactible> booksRead = new List<Interactible>();

    private Interactible openBook;
    public GameObject right;
    public GameObject left;
    private int curPage = 0;
    public void OpenBook(Interactible i)
    {
        if (openBook == null)
        {
            openBook = i;
            player.Pause();
            leftPage.text = i.Page(0);
            rightPage.text = i.Page(1);
            curPage = 0;
            canvas.SetActive(true);
            right.SetActive(curPage + 2 < i.PageCount());
            left.SetActive(false);

            actions.FindActionMap("reading").Enable();
            actions.FindActionMap("gameplay").Disable();
        }
    }
    public void CloseBook(InputAction.CallbackContext context)
    {
        if (openBook != null)
        {
            openBook = null;
            player.Unpause();
            canvas.SetActive(false);

            actions.FindActionMap("reading").Disable();
            actions.FindActionMap("gameplay").Enable();
        }
    }

    public void TurnRight(InputAction.CallbackContext context)
    {
        if (openBook != null && curPage + 2 < openBook.PageCount())
        {
            curPage += 2;
            leftPage.text = openBook.Page(curPage);
            rightPage.text = openBook.Page(curPage + 1);
            right.SetActive(curPage + 2 < openBook.PageCount());
            left.SetActive(true);

            if (!right.activeSelf && !booksRead.Contains(openBook))
            {
                booksRead.Add(openBook);
            }
        }
    }
    public void TurnLeft(InputAction.CallbackContext context)
    {
        if (openBook != null && curPage > 0)
        {
            curPage -= 2;
            leftPage.text = openBook.Page(curPage);
            rightPage.text = openBook.Page(curPage + 1);
            right.SetActive(true);
            left.SetActive(curPage > 0);
        }
    }

    private bool restoringColour = false;
    private float restoringColourTimer = 0.0f;
    public float restoringColourSpeed = 1.0f;
    private bool restored = false;

    public Sprite colourBG;
    public Image bookBG;
    public void RestoreColour()
    {
        if (!restored)
        {
            restoringColourTimer = restoringColourSpeed;
            restoringColour = true;
            bookBG.sprite = colourBG;
            restored = true;
        }
    }

    void OnEnable()
    {
        actions.FindActionMap("gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("gameplay").Disable();
    }
}
