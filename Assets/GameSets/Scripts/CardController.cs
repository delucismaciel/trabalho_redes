using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    Card card;

    public int index;

    [SerializeField]
    Image frente, costa;

    Animator animController;

    public Card Card { get => card; set {
            card = value;
            RefreshCard();
        } 
    }

    private void RefreshCard()
    {
        //Ignorar a cor
        //costa.color = card.color;
        costa.sprite = card.texture;
    }

    private void Start()
    {
        animController = GetComponent<Animator>();
        animController.SetBool("show", false);
    }

    public void onClick()
    {
        GetComponent<Button>().interactable = false;

        if (User.user.isMyTurn)
        {
            //Avisa o adversário que deve girar esta carta
            User.user.SendSocket("card:" + index);
        }

        //Se este é o primeiro card que o user clicou
        if (CardsManager.cardManager.firstClick == null)
        {
            //Seta este como o primeiro
            CardsManager.cardManager.firstClick = this;

            //Executa animation
            animController.SetBool("show", true);

        }
        else
        {
            animController.SetBool("show", true);
            GetComponent<Button>().interactable = true;
            CardsManager.cardManager.firstClick.GetComponent<Button>().interactable = true;

            //Se o primeiro card clicado é diferente do card atual, errou
            if (CardsManager.cardManager.firstClick.Card != this.Card)
            {
                StartCoroutine("Error");
            }
            else //ACERTOU!!
            {
                StartCoroutine("Correct");
            }

        }
    }

    IEnumerator Correct()
    {
        yield return new WaitForSeconds(1);
        SongManager.songManager.PlayGood();
        CardsManager.cardManager.firstClick = null;
        CardsManager.cardManager.hidedCards -= 2;

        if (User.user.isMyTurn)
        {
            User.user.WinPlay();
            User.user.points++;
        }

        if(CardsManager.cardManager.hidedCards == 0)
        {
            User.user.EndGame();
            Debug.Log("ENDGAME");
        }

        Debug.Log("Restantes: " + CardsManager.cardManager.hidedCards);
    }

    IEnumerator Error()
    {       
        yield return new WaitForSeconds(1.3f);
        //Reset o firstclick e desfaz a animação
        CardsManager.cardManager.firstClick.animController.SetBool("show", false);

        CardsManager.cardManager.firstClick = null;
        animController.SetBool("show", false);

        //Executam som de erro
        SongManager.songManager.PlayBad();

        if (CardsManager.cardManager.hidedCards == 0)
        {
            User.user.EndGame();
            Debug.Log("ENDGAME");
        }

        User.user.LosePlay();
    }
}
