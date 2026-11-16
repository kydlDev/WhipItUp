namespace KydlCore.Ui;

/// <summary>
/// What a text of a mod's page IS. KydlCore decides how each role looks (size, colour, font); a mod only names the role
/// (UiKit.TextLabel / Style / Tint / Size) and never sets a size, a colour or a font itself (user 2026-11-16).
/// </summary>
public enum TextRole
{
    /// <summary>Normal text.</summary>
    Body,
    /// <summary>The name of a setting or a switch.</summary>
    Name,
    /// <summary>A value: a number, a chosen option, something that stands out.</summary>
    Value,
    /// <summary>A note or hint in the dimmer colour.</summary>
    Note,
    /// <summary>Small normal text.</summary>
    Small,
    /// <summary>A small note: a column header, a hint under a picture.</summary>
    SmallNote,
    /// <summary>A section title.</summary>
    Section,
    /// <summary>A section title in the dimmer colour (something that is not available).</summary>
    SectionMuted,
    /// <summary>The text of a button.</summary>
    Button,
    /// <summary>A good value: enough of something.</summary>
    Good,
    /// <summary>A bad value: not enough of something.</summary>
    Bad,
    /// <summary>The letter on a chip that offers a colour (see UiKit.ColourChip).</summary>
    Chip,
}
