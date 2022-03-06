using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{

    public Animator panelAnim;
    public Animator gameInfoAnim;
    private static readonly int Out = Animator.StringToHash("Out");
    private static readonly int Property = Animator.StringToHash("Game Over");


    public void OK()
    {
        if(panelAnim != null && gameInfoAnim != null)
        {
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            StartCoroutine(GameStartCo());
        }
        
        
    }
    
    public void GameOver()
    {
        panelAnim.SetBool(Out, false);
        panelAnim.SetBool(Property, true);
    }

    private static IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);
        Board board = FindObjectOfType<Board>();
        board.currentState = Gamestate.move;
    }
}
