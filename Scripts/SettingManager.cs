using Godot;
using System;

public partial class SettingManager : Node
{
    [Export] public NodePath InventoryPanelPath;
    [Export] public NodePath InfoPanelPath;

    private Control _inventoryPanel;
    private Control _infoPanel;

    public override void _Ready()
    {
        _inventoryPanel = GetNode<Control>(InventoryPanelPath);
        _infoPanel = GetNode<Control>(InfoPanelPath);

        _inventoryPanel.Visible = false;
        _infoPanel.Visible = false;

        GD.Print("✅ SettingManager initialized: Inventory & Info panel hidden.");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_inventory"))
        {
            _inventoryPanel.Visible = !_inventoryPanel.Visible;
            GD.Print($"📦 Inventory toggled: {_inventoryPanel.Visible}");
        }

        if (@event.IsActionPressed("toggle_info"))
        {
            _infoPanel.Visible = !_infoPanel.Visible;
            GD.Print($"ℹ️ Info panel toggled: {_infoPanel.Visible}");
        }
    }

    public void AddItem(string itemName)
    {
        if (_inventoryPanel == null)
        {
            GD.PushError("Inventory panel belum di-set!");
            return;
        }

        // Pastikan VBoxContainer ada
        var vbox = _inventoryPanel.GetNodeOrNull<VBoxContainer>("VBoxContainer");
        if (vbox == null)
        {
            GD.PushError("VBoxContainer tidak ditemukan di dalam InventoryPanel!");
            return;
        }

        Label label = new Label();
        label.Text = $"- {itemName}";
        vbox.AddChild(label);

        GD.Print($"🧺 Item ditambahkan ke inventory: {itemName}");
    }

}
