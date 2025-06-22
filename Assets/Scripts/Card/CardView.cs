using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private Renderer cardRenderer;
    [SerializeField] private Material cardMaterialTemplate;

    public Card card;

    private Material runtimeMaterial;

    public string textureProperty = "_CardFront";   // ���� Shader Graph�� Property���� ������ Reference �̸�.

    public void SetCard(Card card, bool isHidden = false)
    {
        this.card = card;
        UpdateVisual(isHidden);
    }

    public void UpdateVisual(bool isHidden)
    {
        if (card == null)
        {
            return;
        }

        if (runtimeMaterial == null)
        {
            runtimeMaterial = new Material(cardMaterialTemplate);
            cardRenderer.material = runtimeMaterial;
        }

        Texture2D tex = isHidden ? GetBackTexture() : GetCardTexture(card);
        runtimeMaterial.SetTexture(textureProperty, tex);
    }

    private Texture2D GetCardTexture(Card card)
    {
        string textureName = $"{card.Rank}_of_{card.Suit}";
        // Resources ���� �Ʒ����� �˻�
        return Resources.Load<Texture2D>($"Textures/PlayingCards/{textureName}");
    }

    private Texture2D GetBackTexture()
    {
        // Resources ���� �Ʒ����� �˻�
        return Resources.Load<Texture2D>($"Textures/PlayingCards/Back");
    }

    public override string ToString()
    {
        return card.ToString(); // "Ace_of_Diamonds" ó�� ��µ�.
    }
}
