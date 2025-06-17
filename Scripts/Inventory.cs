using Godot;
using System.Collections.Generic;

public partial class Inventory : Node
{
    [Export] public NodePath InventoryPanelPath;
    [Export] public PackedScene SlotScene;
    [Export] public int SlotCount = 20;

    private Control _panel;
    private GridContainer _grid;
    private List<Label> _slots = new();

    public override void _Ready()
    {
        _panel = GetNode<Control>(InventoryPanelPath);
        _grid = _panel.GetNode<GridContainer>("GridContainer");
        _panel.Visible = false;

        for (int i = 0; i < SlotCount; i++)
        {
            var slotInstance = SlotScene.Instantiate<Control>();
            _grid.AddChild(slotInstance);

            var label = slotInstance.GetNode<Label>("Label");
            label.Text = ""; // Kosongkan di awal
            _slots.Add(label);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_inventory"))
        {
            _panel.Visible = !_panel.Visible;
        }
    }

    public void AddItem(string itemName)
    {
        foreach (var label in _slots)
        {
            if (label.Text == "")
            {
                label.Text = itemName;
                break;
            }
        }
    }
}
