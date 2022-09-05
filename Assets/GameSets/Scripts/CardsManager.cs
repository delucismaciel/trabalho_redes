using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsManager : MonoBehaviour
{
    public static CardsManager cardManager;
    public CardController firstClick;

    [SerializeField]
    GameObject blocker;

    public int maxQuant = 18;
    public List<Card> possibleCards;
    public List<CardController> cards;

    [SerializeField]
    GameObject prefabCard;

    [SerializeField]
    GameObject grid;

    private void Awake()
    {
        cardManager = GetComponent<CardsManager>();
    }

    private void Start()
    {
        Reorder();
    }

    public void Reorder()
    {
        //Calcula a quantidade máxima de cards na tabela
        int max = Mathf.Min(maxQuant/2, possibleCards.Count)*2;

        Debug.Log(max);

        Card[] cards = new Card[max];

        int i;
        //Sorteio dos cards
        //Ao fim do for, o vetor cards vai ter uma lista de cards únicos sorteados aleatóriamente
        for (i = 0; i< max/2; i++)
        {
            //Pega um elemento aleatório da lista de cards possíveis
            int j = Random.Range(0, possibleCards.Count);

            cards[i] = possibleCards[j];

            //Debug.Log("CARD " + i + ": " + cards[i].id);

            //Remove o card para ele não ser sorteado novamente
            possibleCards.Remove(cards[i]);
        }

        //Clona a primeira metade com exatamente os mesmos cards
        //Assim, teremos 2 cards iguais por game
        for(;i<max; i++)
        {
            cards[i] = cards[i - max / 2];
            //Debug.Log("CARD " + i + ": " + cards[i].id);
        }

        //Reordenar o vetor
        for(i = 0; i<max; i++)
        {
            int j = Random.Range(0, cards.Length);
            Card aux = cards[i];
            cards[i] = cards[j];
            cards[j] = aux; 
        }

        //Cria os cards na UI
        for (i = 0; i < max; i++)
        {
            CardController temp = Instantiate(prefabCard, grid.transform).GetComponent<CardController>();
            temp.Card = cards[i];
        }

    }

    public void Block()
    {
        blocker.SetActive(true);
    }
    public void Unblock()
    {
        blocker.SetActive(false);
    }
}
