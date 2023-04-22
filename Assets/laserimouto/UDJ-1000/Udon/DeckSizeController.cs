
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class DeckSizeController : UdonSharpBehaviour
{
    public Transform decks;
    public InputField deckSizeInput;

    [UdonSynced, FieldChangeCallback(nameof(DeckSize))]
    private int _deckSize;

    private int DeckSize
    {
        get => _deckSize;
        set
        {
            _deckSize = value;
            if (!deckSizeInput.text.Equals(_deckSize.ToString()))
                deckSizeInput.text = _deckSize.ToString();

            decks.localScale = Vector3.one * _deckSize / 100f;
        }
    }

    private void Start()
    {
        DeckSize = 100;
    }

    public void SetOwner()
    {
        Debug.Log("Setting Owner to " + Networking.LocalPlayer);
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void OnInputFieldChanged()
    {
        if (int.TryParse(deckSizeInput.text, out int newSize))
            SetDeckSize(newSize);
    }

    public void SetTinyDeckSize()
    {
        SetDeckSize(30);
    }

    public void SetSmallDeckSize()
    {
        SetDeckSize(60);
    }

    public void SetMediumDeckSize()
    {
        SetDeckSize(90);
    }

    public void SetLargeDeckSize()
    {
        SetDeckSize(120);
    }

    public void DecreaseDeckSize()
    {
        SetDeckSize(DeckSize - 10);
    }

    public void IncreaseDeckSize()
    {
        SetDeckSize(DeckSize + 10);
    }

    private void SetDeckSize(int size)
    {
        if (!Networking.IsOwner(decks.gameObject))
        {
            Debug.Log("dsjgfrljgfdslidjgflk");
            return;
        }

        DeckSize = size;
    }
}
