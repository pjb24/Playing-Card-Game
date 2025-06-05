using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;   // Inspector에서 할당
    public Transform spawnPoint;    // 카드가 생성될 위치

    public int numberOfDeck = 1;

    private Deck deck;

    private void Start()
    {
        deck = new Deck(numberOfDeck);
        SpawnCard();
    }

    void SpawnCard()
    {
        Card card = deck.DrawCard();
        if (card == null)
            return;

        GameObject cardObj = Instantiate(cardPrefab, spawnPoint.position, Quaternion.identity);
        CardView view = cardObj.GetComponent<CardView>();

        Texture2D texture = GetTextureForCard(card);
        view.Initialize(card, texture);
    }

    Texture2D GetTextureForCard(Card card)
    {
        string textureName = $"{card.Rank}_of_{card.Suit}";
        // Resources 폴더 아래에서 검색
        return Resources.Load<Texture2D>($"Textures/PlayingCards/{textureName}");
    }

}
