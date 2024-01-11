using System;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public event Action<string> OnOptionSelected;
    
    public void AddOption(string id, string option)
    {
    }

    public void RemoveAllOptions()
    {
    }
}
