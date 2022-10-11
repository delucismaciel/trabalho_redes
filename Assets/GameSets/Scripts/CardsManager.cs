using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CardsManager : MonoBehaviour
{
    public static CardsManager cardManager;
    public CardController firstClick;

    [SerializeField]
    public TextMeshProUGUI myName, vsName;

    [SerializeField]
    public GameObject underMyName, underVsName, endGamePanel, winImage, loseImage;
   
    [SerializeField]
    GameObject blocker;

    public int maxQuant = 18, hidedCards = 18;
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
        myName.text = User.user.userName;
        if(User.user.type == UserType.host)
        {
            Reorder();
            string order="";
            for(int i = 0; i < cards.Count; i++)
            {
                if (i + 1 == cards.Count)
                    order += cards[i].Card.id;
                else
                {
                    order += cards[i].Card.id + ",";
                }
            }
            User.user.SendSocket("sync:"+order);
            Unblock();
        }
        else
        {
            StartCoroutine("SendName");
            Block();
        }
    }

    IEnumerator SendName()
    {
        yield return new WaitForSeconds(0.2f);
        User.user.SendSocket("name:" + User.user.userName);
    }

    public void Order(int [] order)
    {
        List<CardController> temp = new List<CardController>();
        //Cria os cards na UI
        Debug.Log(order.Length);
        for (int i = 0; i < order.Length; i++)
        {
            Debug.Log(order[i]);
            temp.Add(Instantiate(prefabCard, grid.transform).GetComponent<CardController>());
            temp[i].Card = possibleCards[order[i]-1];
            temp[i].index = i;
        }

        cards = temp;
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

        List<CardController> temp = new List<CardController>();
        //Cria os cards na UI
        for (i = 0; i < max; i++)
        {
            temp.Add(Instantiate(prefabCard, grid.transform).GetComponent<CardController>());
            temp[i].Card = cards[i];
            temp[i].index = i;
        }

        this.cards = temp;
    }

    private void Update()
    {
        if(User.user.isMyTurn)
        {
            underMyName.SetActive(true);
            underVsName.SetActive(false);
        }
        else
        {
            underMyName.SetActive(false);
            underVsName.SetActive(true);
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

    public void MainMenu()
    {
        User.user.CloseRoom();
        Destroy(User.user.gameObject);
        SceneManager.LoadScene(0);
    }
}
