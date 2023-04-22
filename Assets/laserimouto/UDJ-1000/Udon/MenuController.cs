
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class MenuController : UdonSharpBehaviour
{
    public Transform table;

    [UdonSynced, FieldChangeCallback(nameof(Height))]
    private float _height;

    private float Height
    {
        get => _height;
        set
        {
            _height = value;

            table.SetPositionAndRotation(new Vector3(table.position.x, _height, table.position.z), table.rotation);
        }
    }

    public void SetOwner()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void SetSmallTable()
    {
        SetTableHeight(-0.25f);
    }

    public void SetMediumTable()
    {
        SetTableHeight(0f);
    }

    public void SetLargeTable()
    {
        SetTableHeight(0.25f);
    }

    private void SetTableHeight(float height)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        Height = height;
    }
}
