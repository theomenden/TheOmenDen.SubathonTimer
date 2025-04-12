using Microsoft.AspNetCore.Components;
using TheOmenDen.SubathonTimer.Models;
using TheOmenDen.SubathonTimer.Models.Enums;

namespace TheOmenDen.SubathonTimer.Components;

public partial class TimerConfig : ComponentBase
{
    private TimerConfiguration Config = new();
    private string _selectedTheme = "system";

    protected override async Task OnInitializedAsync()
    {
        await ConfigService.LoadAsync();
        Config = ConfigService.Config;
    }

    private void UpdateSubBoost(TwitchSubscriptionTier tier, int value)
    {
        var updatedSubs = Config.SubBoosts.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == tier ? value : kvp.Value);
        Config = Config with { SubBoosts = updatedSubs };
    }

    private int GetBitsBoostValue(BitThreshold valueToSearch)
    {
        return Config.BitsBoostTable.TryGetValue(valueToSearch, out var value) ? value : 0;
    }

    private void UpdateBitsBoost(BitThreshold threshold, int value)
    {
        var updatedBits =
            Config.BitsBoostTable.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == threshold ? value : kvp.Value);
        Config = Config with { BitsBoostTable = updatedBits };
    }

    private void OnThemeChanged(string theme)
    {
        if (String.IsNullOrWhiteSpace(theme) || !Enum.TryParse<FluentTheme>(theme, true, out var parsedResult))
        {
            parsedResult = FluentTheme.System;
            _selectedTheme = "system";
        }

        Config = Config with { Theme = parsedResult };
        _selectedTheme = theme;
    }

    private async Task Save()
    {
        await ConfigService.UpdateAsync(Config).ConfigureAwait(false);
    }
}