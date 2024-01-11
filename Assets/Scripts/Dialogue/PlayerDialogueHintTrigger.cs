using Core;
using UnityEngine;

public class PlayerDialogueHintTrigger : MonoBehaviour
{
    [SerializeField] private GameObject hintView;
    private bool _hasPlayer;
    private bool _wasHintActive;
 
    private void Update()
    {
        bool hintActive = _hasPlayer && Game.Overworld.PlayerDialogue.HasOptions();
        
        if (hintActive && !_wasHintActive)
        {
            // just got hint
            hintView.SetActive(true);
        }
        if (!hintActive && _wasHintActive)
        {
            // just lost hint 
            hintView.SetActive(false);
        }

        _wasHintActive = hintActive;
        
        if (hintActive && Input.GetKeyDown(KeyCode.Q))
            Game.Overworld.PlayerDialogue.Open();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponentWithRigidbody(out OverworldPlayerView player))
            _hasPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponentWithRigidbody(out OverworldPlayerView player))
            _hasPlayer = false;
    }
}
