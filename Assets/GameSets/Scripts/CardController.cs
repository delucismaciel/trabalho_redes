using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    Card card;

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
        //Se este é o primeiro card que o user clicou
        if (CardsManager.cardManager.firstClick == null)
        {
            //Seta este como o primeiro
            CardsManager.cardManager.firstClick = this;

            //Executa animation
            animController.SetBool("show", true);

            //Avisa o adversário que deve girar esta carta
            //AQUI
        }
        else
        {
            animController.SetBool("show", true);

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


        //Avisa que acertou
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

        User.user.isMyTurn = false;
        User.user.LosePlay();
    }
}
