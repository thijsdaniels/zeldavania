using Godot;
using Zeldavania.Combat;

[GlobalClass]
public partial class LightEmitter2D : PointLight2D
{
    [Export]
    public Color LightColor { get; set; } = new Color(0.82f, 0.51f, 0.14f, 1f);

    [Export]
    public float BaseEnergy { get; set; } = 0.6f;

    [Export]
    public float RadiusScale { get; set; } = 0.6f;

    [Export]
    public bool Flicker { get; set; } = true;

    [Export]
    public float FlickerIntensity { get; set; } = 0.08f;

    [Export]
    public float FlickerSpeed { get; set; } = 12f;

    [Export]
    public bool AutoBindToFlammable { get; set; } = true;

    [Export]
    private Flammable2D _flammable;

    private float _flickerTime = 0f;

    public override void _Ready()
    {
        Color = LightColor;
        Energy = BaseEnergy;
        TextureScale = RadiusScale;
        ShadowEnabled = true;

        if (Texture == null)
        {
            Texture = GD.Load<Texture2D>("res://assets/textures/PointLight.webp");
        }

        if (_flammable == null && AutoBindToFlammable)
        {
            _flammable = FindSiblingOrParentFlammable();
        }

        if (_flammable != null)
        {
            _flammable.Ignited += OnFlammableIgnited;
            _flammable.Extinguished += OnFlammableExtinguished;
            _flammable.BurnCompleted += OnFlammableBurnCompleted;

            SetLightActive(_flammable.IsBurning);
        }
    }

    public override void _ExitTree()
    {
        if (_flammable != null)
        {
            _flammable.Ignited -= OnFlammableIgnited;
            _flammable.Extinguished -= OnFlammableExtinguished;
            _flammable.BurnCompleted -= OnFlammableBurnCompleted;
        }
    }

    public override void _Process(double delta)
    {
        if (!Visible || !Enabled)
        {
            return;
        }

        if (Flicker)
        {
            _flickerTime += (float)delta * FlickerSpeed;
            float noise =
                (Mathf.Sin(_flickerTime) * 0.5f)
                + (Mathf.Sin(_flickerTime * 2.37f) * 0.3f)
                + (Mathf.Sin(_flickerTime * 5.13f) * 0.2f);

            Energy = Mathf.Max(0f, BaseEnergy + (noise * FlickerIntensity));
            TextureScale = Mathf.Max(
                0.05f,
                RadiusScale + (noise * (FlickerIntensity * 0.5f))
            );
        }
    }

    public void SetLightActive(bool active)
    {
        Visible = active;
        Enabled = active;
    }

    private Flammable2D FindSiblingOrParentFlammable()
    {
        var parent = GetParent();
        if (parent == null)
        {
            return null;
        }

        if (parent is Flammable2D parentFlammable)
        {
            return parentFlammable;
        }

        foreach (Node child in parent.GetChildren())
        {
            if (child is Flammable2D childFlammable)
            {
                return childFlammable;
            }
        }

        var grandparent = parent.GetParent();
        if (grandparent != null)
        {
            foreach (Node child in grandparent.GetChildren())
            {
                if (child is Flammable2D childFlammable)
                {
                    return childFlammable;
                }
            }
        }

        return null;
    }

    private void OnFlammableIgnited()
    {
        SetLightActive(true);
    }

    private void OnFlammableExtinguished()
    {
        SetLightActive(false);
    }

    private void OnFlammableBurnCompleted()
    {
        if (_flammable != null)
        {
            SetLightActive(_flammable.IsBurning);
        }
    }
}
