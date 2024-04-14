using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public Player player;
    public GameObject canvas;
    public TextMeshProUGUI leftPage;
    public TextMeshProUGUI rightPage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(openBook != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                CloseBook();
            }
        }
    }

    private Interactible openBook;

    public void OpenBook(Interactible i)
    {
        openBook = i;
        player.Pause();
        leftPage.text = i.Page(0);
        rightPage.text = i.Page(1);
        canvas.SetActive(true);
    }
    void CloseBook()
    {
        openBook = null;
        player.Unpause();
        canvas.SetActive(false);
    }
}
