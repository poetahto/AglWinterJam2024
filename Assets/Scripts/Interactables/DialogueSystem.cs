using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private DialogueOptionView optionViewPrefab;
    [SerializeField] private Transform optionViewParent;
    [SerializeField] private GameObject dialogueViewObject;
    
    public event Action<string> OnOptionSelected;

    private readonly List<DialogueOptionView> _options = new List<DialogueOptionView>();
    private bool _isOpen;

    private void Start()
    {
        dialogueViewObject.SetActive(false);
    }

    private void Update()
    {
        if (_isOpen && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public bool IsOpen()
    {
        return _isOpen;
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
        DialogueOptionView instance = Instantiate(optionViewPrefab, optionViewParent);
        instance.text.SetText(option);
        instance.button.onClick.AddListener(() => HandleOptionSelected(id));
        _options.Add(instance);
    }
    
    public void RemoveAllOptions()
    {
        foreach (DialogueOptionView option in _options)
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
