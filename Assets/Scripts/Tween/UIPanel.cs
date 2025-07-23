using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPanel : MonoBehaviour
{
    public Button[] button;

    private GameObject selected;
    private EventSystem eventSystem;

    void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(button[0].gameObject);
            eventSystem.firstSelectedGameObject = button[0].gameObject;
            selected = button[0].gameObject;

            // Adiciona o evento de foco para cada botão
            foreach (var btn in button)
            {
                AddEventTrigger(btn);
            }
        }
        else
        {
            Debug.LogWarning("Event System is null");
        }
    }

    private void AddEventTrigger(Button btn)
    {
        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>() ?? btn.gameObject.AddComponent<EventTrigger>();

        // Cria uma entrada de evento para OnPointerEnter
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };

        // Associa o método OnButtonHover
        entry.callback.AddListener((data) => OnButtonHover(btn));
        trigger.triggers.Add(entry);
    }

    private void OnButtonHover(Button btn)
    {
        // Define o botão como selecionado quando o mouse passa sobre ele
        eventSystem.SetSelectedGameObject(btn.gameObject);
        selected = btn.gameObject;
    }
}
