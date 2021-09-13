#r "C:\Program Files\workspacer\workspacer.Shared.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.Bar\workspacer.Bar.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.Gap\workspacer.Gap.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.TitleBar\workspacer.TitleBar.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"

using System;
using workspacer;
using workspacer.Bar;
using workspacer.Gap;
using workspacer.TitleBar;
using workspacer.ActionMenu;
using workspacer.FocusIndicator;

Action<IConfigContext> doConfig = (context) =>
{
    context.CanMinimizeWindows = false;

    var fontSize = 11;
    var fontFace = "JetBrainsMono NF";
    var taskBarSize = fontSize * 2;
    var defaultBgColor = new Color( 0x26, 0x26, 0x26);
    var gap = 8;

    context.AddBar( new BarPluginConfig(){
        FontSize = fontSize,
        FontName = fontFace,
        BarHeight = taskBarSize,
        DefaultWidgetBackground = defaultBgColor
    });

    var gapPlugin = context.AddGap(
        new GapPluginConfig() { InnerGap = gap, OuterGap = gap / 2, Delta = gap / 2 }
    );

    var titleBarPluginConfig = new TitleBarPluginConfig(
        new TitleBarStyle(
            showTitleBar: false,
            showSizingBorder: false
    ));
    context.AddTitleBar(titleBarPluginConfig);

    context.AddFocusIndicator(
        new FocusIndicatorPluginConfig() {
            BorderColor = new Color(0x02, 0xF8, 0xFF),
            BorderSize = 4
    });
    var actionMenu = context.AddActionMenu();

    context.DefaultLayouts = () => new ILayoutEngine[] {
        new TallLayoutEngine(),
        new FullLayoutEngine()
    };
    context.WorkspaceContainer.CreateWorkspaces("1", "2");

    Action setKeyBindings = () => {
        KeyModifiers win = KeyModifiers.Win;
        KeyModifiers alt = KeyModifiers.Alt;
        var modKey = alt;
        KeyModifiers modShift = modKey | KeyModifiers.Shift;

        IKeybindManager manager = context.Keybinds;

        manager.UnsubscribeAll();

        var workspaces = context.Workspaces;
        manager.Subscribe(modKey, Keys.H, () => workspaces.SwitchFocusToNextMonitor());
        manager.Subscribe(modKey, Keys.J, () => {
                workspaces.FocusedWorkspace.FocusNextWindow();
                // workspaces.SwitchFocusToNextMonitor();
        });
        manager.Subscribe(modKey, Keys.K, () => {
                workspaces.FocusedWorkspace.FocusPreviousWindow();
                // workspaces.SwitchFocusToNextMonitor();
        });
        manager.Subscribe(modKey, Keys.L, () => workspaces.SwitchFocusToPreviousMonitor());

        manager.Subscribe(modShift, Keys.H, () => workspaces.MoveFocusedWindowToNextMonitor());
        manager.Subscribe(modShift, Keys.J, () => workspaces.FocusedWorkspace.SwapFocusAndNextWindow());
        manager.Subscribe(modShift, Keys.K, () => workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow());
        manager.Subscribe(modShift, Keys.L, () => workspaces.MoveFocusedWindowToPreviousMonitor());

        manager.Subscribe(modKey, Keys.Oemcomma,  () => workspaces.FocusedWorkspace.ShrinkPrimaryArea(), "shrink primary area");
        manager.Subscribe(modKey, Keys.OemPeriod, () => workspaces.FocusedWorkspace.ExpandPrimaryArea(), "expand primary area");

        manager.Subscribe(modKey, Keys.Space, () => workspaces.FocusedWorkspace.NextLayoutEngine());
        manager.Subscribe(modKey, Keys.Return, () => workspaces.FocusedWorkspace.SwapFocusAndPrimaryWindow(), "make current window primary" );

        manager.Subscribe(modKey, Keys.C, () => workspaces.FocusedWorkspace.CloseFocusedWindow());
        manager.Subscribe(modKey, Keys.I, () => context.ToggleConsoleWindow(), "toggle debug console");
        manager.Subscribe(modKey, Keys.Q, () => context.Quit());
        manager.Subscribe(modShift, Keys.Q, () => context.Restart());
    };
    setKeyBindings();
};
return doConfig;