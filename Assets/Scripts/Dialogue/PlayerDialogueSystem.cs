using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueSystem : MonoBehaviour
{
    [SerializeField] private PlayerDialogueOptionView optionViewPrefab;
    [SerializeField] private Transform optionViewParent;
    [SerializeField] private GameObject dialogueViewObject;
    
    public event Action<string> OnOptionSelected;

    private readonly List<PlayerDialogueOptionView> _options = new List<PlayerDialogueOptionView>();
    private bool _isOpen;

    private void Start()
    {
        dialogueViewObject.SetActive(false);
    }

    private void Update()
    {
        if (_isOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)))
            Close();
    }

    public bool HasOptions()
    {
        return _options.Count > 0;
    }

    public void Open()
    {
        if (!HasOptions())
            throw new Exception("Tried to open dialogue without options!");
        
        dialogueViewObject.SetActive(true);
        Game.Overworld.Player.inputObject.SetActive(false);
        _isOpen = true;
    }

    public void Close()
    {
        dialogueViewObject.SetActive(false);
        Game.Overworld.Player.inputObject.SetActive(true);
        _isOpen = false;
    }

    public void AddOption(string id, string option)
    {
        PlayerDialogueOptionView instance = Instantiate(optionViewPrefab, optionViewParent);
        instance.text.SetText(option);
        instance.button.onClick.AddListener(() => HandleOptionSelected(id));
        _options.Add(instance);
    }
    
    public void RemoveAllOptions()
    {
        foreach (PlayerDialogueOptionView option in _options)
            Destroy(option.gameObject);
        
        _options.Clear();
        Close();
    }

    private void HandleOptionSelected(string id)
    {
        OnOptionSelected?.Invoke(id);
        RemoveAllOptions();
    }
}
