using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Text Puntosplayer;
    public Text Puntosdealer;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;   
    public Text probMessage_1;
    public Text probMessage_2;

    public List<int> cardOrders = new List<int>();
    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        cardIndex = 0;
        
        for(int i = 0; i < values.Length; i++)
        {
            if (cardIndex==0)
            {
                values[i] = 11;
                cardIndex += 1;
            }
            else if (cardIndex <10)
            {
                values[i] = cardIndex + 1;
                cardIndex += 1;
            }
            else if(cardIndex<13)
            {
                values[i] = 10;
                if (cardIndex >= 12)
                {
                    cardIndex = 0;
                }
                else
                {
                    cardIndex += 1;
                }
            }
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        for (int i = 0; i < 52; i++)
        {
            int rand = Random.Range(0, 52);
            while (cardOrders.Contains(rand))
            {
                rand = Random.Range(0, 52);
            }
            cardOrders.Add(rand);
        }

    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();

            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }

        Puntosplayer.text = player.GetComponent<CardHand>().points.ToString();
        Puntosdealer.text = dealer.GetComponent<CardHand>().points.ToString();

        if (dealer.GetComponent<CardHand>().points == 21 || player.GetComponent<CardHand>().points == 21)
        {
            hitButton.interactable = false;
            stickButton.interactable = false;
            finalMessage.text = "Fin del Juego.  Se ha obtenido BlackJack!";
        }

        CalculateProbabilities();

    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */

        dealerMorePlayer();
        //Debug.Log("Probabilidad de que el dealer tenga mas que tu " + prob1);
        sePasa();
        entreRango();
    }
    public double dealerMorePlayer()
    {

        int casosFavorables = 0;
        int[] cartasMesa = new int[3];

        int puntuacionJugador = player.GetComponent<CardHand>().points;
        int cartaDealer = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;
        int diferencia = puntuacionJugador - cartaDealer;

        cartasMesa[0] = player.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().value;
        cartasMesa[1] = player.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;
        cartasMesa[2] = cartaDealer;

        if (diferencia < 0)
        {
            probMessage.text = 0.ToString();
            return 0;
        }

        /*
        int puntuacionJugador = 7;
        int cartaDealer = 5;
        int diferencia = puntuacionJugador - cartaDealer;

        cartasMesa[0] = 3;
        cartasMesa[1] = 4;
        cartasMesa[2] = cartaDealer;
        */
        int valorCarta;

        for (int i = diferencia + 1; i < 12; i++)
        {
            int contadorCartas = 0;

            if (i == cartasMesa[0])
            {
                contadorCartas++;
            }
            if (i == cartasMesa[1])
            {
                contadorCartas++;
            }
            if (i == cartasMesa[2])
            {
                contadorCartas++;
            }

            //Debug.Log("Contador cartas " + contadorCartas);
            if (i != 10)
                casosFavorables = casosFavorables + (4 - contadorCartas);

            if (i == 10)
            {
                casosFavorables = casosFavorables + (16 - contadorCartas);
            }
        }

        Debug.Log("Casos favorables Mas que dealer " + casosFavorables);

        float probabilidad = (float)casosFavorables / 49;
        probabilidad = 1 - probabilidad;
        probabilidad = probabilidad * 100;
        probMessage.text = probabilidad.ToString();
        return probabilidad;
    }

    public float entreRango()
    {
        int casosFavorables = 0;

        int puntuacionJugador = player.GetComponent<CardHand>().points;
        int cartasRepartidas = player.GetComponent<CardHand>().cards.Count + 1;

        int[] cartasMesa = new int[cartasRepartidas];

        for (int i = 0; i < cartasRepartidas - 2; i++)
        {
            cartasMesa[i] = player.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value;
        }

        cartasMesa[cartasRepartidas - 1] = player.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;

        int cartaMinima = 17 - puntuacionJugador;
        int cartaMaxima = 21 - puntuacionJugador;

        if (cartaMinima <= 0)
        {
            cartaMinima = 1;
        }

        if (cartaMaxima <= 0)
        {
            Debug.Log("Probabilidad de al pedir carta estar entre rango 0%");
            return 0;
        }

        if (cartaMinima >= 11)
        {
            Debug.Log("Probabilidad de al pedir carta estar entre rango 100%");
            return 100;
        }

        for (int i = cartaMinima; i < cartaMaxima + 1; i++)
        {
            int contador = 0;
            for (int j = 0; j < cartasMesa.Length; j++)
            {
                if (i == cartasMesa[j])
                {
                    contador++;
                }
            }
            if (i != 10)
            {
                casosFavorables += (4 - contador);
            }
            else
            {
                casosFavorables += (16 - contador);
            }
        }

        Debug.Log("Casos favorables Entre rango " + casosFavorables);
        float probabilidad = (float)casosFavorables / 49;
        probabilidad = probabilidad * 100;
        Debug.Log("Probabilidad entre rango " + probabilidad);
        probMessage_1.text = probabilidad.ToString();
        return probabilidad;
    }

    public float sePasa()
    {
        int casosFavorables = 0;
        int cartasRepartidas = player.GetComponent<CardHand>().cards.Count + 1;
        int[] cartasMesa = new int[cartasRepartidas];

        int puntuacionJugador = player.GetComponent<CardHand>().points;
        int cartaDealer = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;

        cartasMesa[0] = player.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().value;
        cartasMesa[1] = player.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;
        cartasMesa[2] = cartaDealer;

        if (puntuacionJugador == 21)
        {
            Debug.Log("Probabilidad de pasarte de 100%");
            return 100;
        }

        int diferencia = 21 - puntuacionJugador;

        if (diferencia > 11)
        {
            Debug.Log("Probabilidad de pasarte del 0%");
            return 0;
        }

        for (int i = 1; i < diferencia + 1; i++)
        {
            int contadorCartas = 0;

            if (i == cartasMesa[0])
            {
                contadorCartas++;
            }
            if (i == cartasMesa[1])
            {
                contadorCartas++;
            }
            if (i == cartasMesa[2])
            {
                contadorCartas++;
            }

            if (i != 10)
            {
                casosFavorables += (4 - contadorCartas);

            }
            else
            {
                casosFavorables  += (16 - contadorCartas);
            }

        }

        Debug.Log("casos favorables Pasarte " + casosFavorables);
        float probabilidad = (float)casosFavorables / 49;
        probabilidad = 1 - probabilidad;
        probabilidad = probabilidad * 100;
        Debug.Log("Probabilidad de pasarte " + probabilidad);
        probMessage_2.text = probabilidad.ToString();
        return probabilidad;
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardOrders[cardIndex]],values[cardOrders[cardIndex]]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardOrders[cardIndex]], values[cardOrders[cardIndex]]/*,cardCopy*/);
        cardIndex++;
        //CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        //Repartimos carta al jugador
        PushPlayer();
        CalculateProbabilities();
        

        Puntosplayer.text = player.GetComponent<CardHand>().points.ToString();
        /*TODO:
        * Comprobamos si el jugador ya ha perdido y mostramos mensaje
        */
        if (player.GetComponent<CardHand>().points > 21)
        {
            hitButton.interactable = false;
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            finalMessage.text = "Has perdido";
        }
       

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        hitButton.interactable = false;

        //damos la vuelta a la carta del dealer
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);


        while (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
            Puntosdealer.text = dealer.GetComponent<CardHand>().points.ToString();
            //Debug.Log(dealer.GetComponent<CardHand>().points);
            if (dealer.GetComponent<CardHand>().points > 21)
            {
                finalMessage.text = "Has ganado, el dealer se ha pasado";
                stickButton.interactable = false;
                
                return;
            }
        }

        if (dealer.GetComponent<CardHand>().points == player.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Empate";
            stickButton.interactable = false;
        }

        if (dealer.GetComponent<CardHand>().points < player.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Has Ganado, tienes mas puntos que el dealer";
            stickButton.interactable = false;
        }
        else
        {
            finalMessage.text = "Has Perdido, el dealer tiene mas puntos";
            stickButton.interactable = false;
        }
        Puntosdealer.text = dealer.GetComponent<CardHand>().points.ToString();
    }

    public void PlayAgain()
    {

        for (int i = 0; i < 52; i++)
        {
            cardOrders[i] = 0;
        }
        cardOrders.Clear();
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
}
