#r "C:\Program Files\workspacer\workspacer.Shared.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.Bar\workspacer.Bar.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.Gap\workspacer.Gap.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.TitleBar\workspacer.TitleBar.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
#r "C:\Program Files\workspacer\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"

using System;
using System.Diagnostics;
using workspacer;
using workspacer.Bar;
using workspacer.Bar.Widgets;
using workspacer.Gap;
using workspacer.TitleBar;
using workspacer.ActionMenu;
using workspacer.FocusIndicator;

Action<IConfigContext> doConfig = (context) =>
{
    context.CanMinimizeWindows = false;

    var fontSize = 12;
    var fontFace = "JetBrainsMono NF";
    var taskBarSize = (int) (fontSize * 2.1);
    var defaultBgColor = new Color( 0x22, 0x25, 0x22);
    var gap = 10;

    context.AddBar( new BarPluginConfig(){
        FontSize = fontSize,
        FontName = fontFace,
        BarHeight = taskBarSize,
        DefaultWidgetBackground = defaultBgColor,
        LeftWidgets = () => new IBarWidget[] {
            new TextWidget(" "),
            new TitleWidget() {
                MonitorHasFocusColor = new Color(0x02, 0xF0, 0xFF),
            }
        },
        RightWidgets = () => new IBarWidget[] {
            new TimeWidget(1000, "HH:mm:ss - dd.MM.yyyy"),
            new TextWidget(" | "),
            new ActiveLayoutWidget(),
            new TextWidget(" ")
        }
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
            BorderColor = new Color(0xF8, 0x02, 0xFF),
            BorderSize = 5,
            GapInset = 5
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

        // Launching apps:

        // windowsTerminal
        manager.Subscribe(modKey, Keys.T, () => Process.Start("wt.exe", ""));
        // launching brave through cmd
        var braveStartInfo = new ProcessStartInfo{
            FileName = "cmd.exe",
            Arguments = "/C start brave",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = true
        };
        manager.Subscribe(modKey, Keys.B, () => Process.Start(braveStartInfo));
        manager.Subscribe(modKey, Keys.E, () => Process.Start("explorer.exe", ""));
    };
    setKeyBindings();
};
return doConfig;
