using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSine : MonoBehaviour
{
    [Header("Configurações do movimento vertical")]
    public float amplitudeY = 10f;    // Quanto vai subir/descer (em pixels)
    public float frequencyY = 2f;     // Velocidade vertical

    [Header("Configurações do movimento horizontal")]
    public float amplitudeX = 5f;     // Quanto vai para os lados (em pixels)
    public float frequencyX = 1.5f;   // Velocidade horizontal
    public float phaseOffsetX = Mathf.PI / 2f; // Defasagem (desincronização)

    private RectTransform rectTransform;
    private Vector2 startPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition; // posição inicial
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequencyY) * amplitudeY;
        float xOffset = Mathf.Sin(Time.time * frequencyX + phaseOffsetX) * amplitudeX;

        rectTransform.anchoredPosition = startPos + new Vector2(xOffset, yOffset);
    }
}

