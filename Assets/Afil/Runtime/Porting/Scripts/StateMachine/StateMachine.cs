using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public static StateMachine current;
    
   public PAGE curPage;
    PAGE lastPage;
    PAGE tempPage;
   
    private void Awake()
    {
        if(current != null)
        {
            return;
        }

        current = this;
        
    }

    // Update is called once per frame
    public void UpdatePage(PAGE page)
    {

        lastPage = curPage;
        curPage = page;
    }

    public void SetLoadingPage()
    {
        if (!OnTheState(PAGE.LOADING))
        {
            tempPage = curPage;
            curPage = PAGE.LOADING;
        }
    }
    public void EndLoadingPage()
    {
        if (OnTheState(PAGE.LOADING))
        {
            curPage = tempPage;
            tempPage = PAGE.NULL;
        }
    }

    public bool OnTheState(params PAGE[] pages)
    {
        foreach(PAGE p in pages)
        {
            if(p == curPage)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsLastPage(params PAGE[] pages)
    {

        foreach(PAGE p in pages)
        {
            if(p == lastPage)
            {
                return true;
            }
        }

        return false;
    }
    public enum PAGE
    {
        NULL=-1,
        PRESS_BUTTON,
        MAIN_MENU,
        OPTIONS,
        CREDITS,
        GAMEPLAY,
        PAUSE,
        LOADING,
        TUTORIAL,
        CARD_SCREEN
    }
}
