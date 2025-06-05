using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public Card cardData;

    public Renderer cardRenderer;   // Inspector���� ����.
    public string textureProperty = "_CardFront";   // ���� Shader Graph�� Property���� ������ Reference �̸�.

    public void Initialize(Card card, Texture2D texture)
    {
        cardData = card;
        SetCardTexture(texture);
    }

    public void SetCardTexture(Texture2D newTexture)
    {
        if (cardRenderer == null
            && newTexture == null)
        {
            Debug.LogWarning("Renderer or texture missing!");
            return;
        }

        cardRenderer.material.SetTexture(textureProperty, newTexture);
    }

    public override string ToString()
    {
        return cardData.ToString(); // "Ace_of_Diamonds" ó�� ��µ�.
    }
}
