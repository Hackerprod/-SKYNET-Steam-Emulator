using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Overlay.Core;

internal sealed class OverlayWindow : Form
{
    private const int PreferredCardWidth = 900;
    private const int PreferredCardHeight = 620;
    private const int PreferredContentWidth = 820;

    public event Action Hidden;

    private readonly OverlayOptions _options;
    private readonly System.Windows.Forms.Timer _positionTimer;
    private readonly RoundedPanel _card;
    private readonly Label _titleLabel;
    private readonly Panel _contentHost;

    public OverlayWindow(OverlayOptions options)
    {
        _options = options ?? new OverlayOptions();

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        BackColor = Theme.BgBase;
        Opacity = 1.0;
        TopMost = _options.TopMost;
        KeyPreview = true;
        DoubleBuffered = true;
        Font = new Font(Theme.FontName, 9f, FontStyle.Regular, GraphicsUnit.Point);
        Bounds = new Rectangle(-32000, -32000, 1, 1);

        _card = new RoundedPanel
        {
            Radius = 12,
            BorderColor = Theme.Border,
            FillColor = Theme.BgCard,
            Size = new Size(PreferredCardWidth, PreferredCardHeight)
        };

        var titleBar = new Panel
        {
            BackColor = Theme.BgDark,
            Dock = DockStyle.Top,
            Height = 58
        };

        _titleLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            ForeColor = Theme.TextTitle,
            Font = new Font(Theme.FontName, 12f, FontStyle.Bold, GraphicsUnit.Point),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(22, 0, 0, 0)
        };

        var closeHost = new Panel
        {
            Dock = DockStyle.Right,
            Width = 58,
            BackColor = Theme.BgDark,
            Padding = new Padding(12)
        };

        var close = new Button
        {
            Text = "X",
            Width = 34,
            Height = 34,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Theme.TextMuted,
            BackColor = Theme.BgDark,
            Font = new Font(Theme.FontName, 9f, FontStyle.Bold, GraphicsUnit.Point),
            Cursor = Cursors.Hand,
            Dock = DockStyle.None,
            Location = new Point(12, 12),
            Size = new Size(34, 34)
        };
        close.FlatAppearance.BorderColor = Theme.BorderSoft;
        close.FlatAppearance.MouseOverBackColor = Theme.BgPanel;
        close.Click += (sender, args) => HideOverlay();
        closeHost.Controls.Add(close);

        titleBar.Controls.Add(_titleLabel);
        titleBar.Controls.Add(closeHost);

        _contentHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.BgCard,
            Padding = new Padding(24, 18, 24, 24)
        };

        _card.Controls.Add(_contentHost);
        _card.Controls.Add(titleBar);
        Controls.Add(_card);

        _positionTimer = new System.Windows.Forms.Timer { Interval = 120 };
        _positionTimer.Tick += (sender, args) => PositionOverTarget();

        MouseDown += (sender, args) =>
        {
            if (!_card.Bounds.Contains(args.Location))
            {
                HideOverlay();
            }
        };

        KeyDown += (sender, args) =>
        {
            if (args.KeyCode == Keys.Escape)
            {
                HideOverlay();
            }
        };
    }

    public void ShowOverlay(OverlayRequest request)
    {
        request ??= new OverlayRequest { Kind = OverlayKind.Home, Title = "SKYNETEMU" };

        PositionOverTarget();
        _card.PerformLayout();
        _contentHost.PerformLayout();

        _titleLabel.Text = ResolveWindowTitle(request);
        _contentHost.Controls.Clear();
        var page = BuildPage(request);
        _contentHost.Controls.Add(page);
        _contentHost.PerformLayout();
        page.PerformLayout();
        UpdateScrollMode(page);

        Show();
        BringToFront();
        Activate();
        _positionTimer.Start();
    }

    public void HideOverlay()
    {
        _positionTimer.Stop();
        Hide();
        Hidden?.Invoke();
    }

    private Control BuildPage(OverlayRequest request)
    {
        var body = CreateScrollBody();

        switch (request.Kind)
        {
            case OverlayKind.UserProfile:
                BuildProfile(body, request);
                break;
            case OverlayKind.UserChat:
                BuildUserChat(body, request);
                break;
            case OverlayKind.UserStats:
                BuildStats(body, request);
                break;
            case OverlayKind.UserAchievements:
                BuildAchievements(body, request);
                break;
            case OverlayKind.People:
                BuildPeople(body, request, "PEOPLE");
                break;
            case OverlayKind.Invite:
                BuildInvite(body, request);
                break;
            case OverlayKind.Store:
                BuildStore(body, request);
                break;
            case OverlayKind.WebPage:
                BuildWebPage(body, request);
                break;
            case OverlayKind.Settings:
                BuildSettings(body, request);
                break;
            case OverlayKind.ConfirmAction:
                BuildConfirm(body, request);
                break;
            default:
                BuildHome(body, request);
                break;
        }

        return body;
    }

    private void BuildHome(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "DASHBOARD");
        AddTitle(body, request.Title ?? "SKYNETEMU", 22f);
        AddSummaryGrid(body, request.Summary);
        AddSectionLabel(body, "RECENT ACTIVITY");

        if (request.Activities.Count == 0)
        {
            AddEmpty(body, "No recent activity has been received from the emulator.");
            return;
        }

        foreach (var activity in request.Activities.Take(8))
        {
            AddInfoCard(body, activity.Title, activity.Detail, Theme.Accent);
        }
    }

    private void BuildProfile(FlowLayoutPanel body, OverlayRequest request)
    {
        var user = request.User ?? new OverlayUser { SteamId = request.UserId ?? 0 };
        var displayName = DisplayName(user, request.Title ?? "User");
        var width = GetContentWidth();
        var status = string.IsNullOrWhiteSpace(user.Status)
            ? (user.PersonaState != 0 ? "Online" : "Offline")
            : user.Status;
        var chipStatus = CompactStatus(status, user.LobbyId);

        var hero = new RoundedPanel
        {
            FillColor = Theme.BgPanel,
            BorderColor = Theme.Border,
            Radius = 12,
            Width = width,
            Height = 146,
            Margin = new Padding(0, 0, 0, 12)
        };

        var avatar = new AvatarControl
        {
            Bounds = new Rectangle(24, 22, 102, 102),
            DisplayName = displayName,
            AvatarPng = user.AvatarPng,
            Online = user.PersonaState != 0
        };
        hero.Controls.Add(avatar);

        hero.Controls.Add(new Label
        {
            Text = displayName,
            ForeColor = Theme.TextTitle,
            Font = new Font(Theme.FontName, 21f, FontStyle.Bold, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(150, 24),
            Size = new Size(Math.Max(180, width - 186), 38)
        });

        hero.Controls.Add(new Label
        {
            Text = user.SteamId == 0 ? "SteamID unavailable" : user.SteamId.ToString(),
            ForeColor = Theme.TextMuted,
            Font = new Font(Theme.FontName, 9.5f, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(152, 64),
            Size = new Size(Math.Max(180, width - 188), 22)
        });

        var chipX = 152;
        chipX += AddChip(hero, user.IsSelf ? "Current user" : RelationshipLabel(user), chipX, 96, user.IsSelf || user.HasFriend ? Theme.Success : Theme.TextMuted) + 8;
        AddChip(hero, chipStatus, chipX, 96, user.PersonaState != 0 ? Theme.Success : Theme.Warning);

        body.Controls.Add(hero);

        AddSectionLabel(body, "PROFILE");
        var facts = new List<Tuple<string, string>>();
        facts.Add(Tuple.Create("AccountID", user.AccountId == 0 ? "Unavailable" : user.AccountId.ToString()));
        if (!user.IsSelf)
        {
            facts.Add(Tuple.Create("Relationship", RelationshipLabel(user)));
        }
        if (user.AppId != 0)
        {
            facts.Add(Tuple.Create(user.IsSelf ? "Current App" : "App", user.AppId.ToString()));
        }
        if (user.LobbyId != 0)
        {
            facts.Add(Tuple.Create("Lobby", user.LobbyId.ToString()));
        }

        AddFactGrid(body, facts);

        var earned = request.Achievements.Count(a => a.Earned);
        var total = request.Achievements.Count;
        var profileSummary = new List<OverlaySummaryItem>
        {
            new OverlaySummaryItem { Label = "Stats", Value = request.Stats.Count.ToString(), Tone = "accent" },
            new OverlaySummaryItem { Label = "Achievements", Value = earned + "/" + total, Tone = earned > 0 ? "success" : "accent" }
        };
        AddSummaryGrid(body, profileSummary);

        var richPresence = FilterProfileRichPresence(user);
        if (richPresence.Count > 0)
        {
            AddSectionLabel(body, "RICH PRESENCE");
            foreach (var item in richPresence.OrderBy(kv => kv.Key).Take(8))
            {
                AddInfoCard(body, item.Key, item.Value, Theme.Accent);
            }
        }
    }

    private void BuildPeople(FlowLayoutPanel body, OverlayRequest request, string label)
    {
        AddSectionLabel(body, label);
        if (request.Users.Count == 0)
        {
            AddEmpty(body, "No users are available in the current emulator cache.");
            return;
        }

        foreach (var user in request.Users)
        {
            AddUserRow(body, user);
        }
    }

    private void BuildInvite(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "AVAILABLE FRIENDS");
        if (request.Users.Count == 0)
        {
            AddEmpty(body, "No friends are available to invite.");
            return;
        }

        foreach (var user in request.Users)
        {
            AddInviteUserRow(body, user, request.InviteUserAction);
        }
    }

    private void BuildStats(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "STATS");
        AddTitle(body, DisplayName(request.User, request.UserId?.ToString() ?? "User"), 20f);
        if (request.Stats.Count == 0)
        {
            AddEmpty(body, "No stats are available for this user yet.");
            return;
        }

        foreach (var stat in request.Stats)
        {
            AddInfoCard(body, stat.Name, string.IsNullOrWhiteSpace(stat.DisplayValue) ? stat.Value.ToString() : stat.DisplayValue, Theme.Accent);
        }
    }

    private void BuildAchievements(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "ACHIEVEMENTS");
        AddTitle(body, DisplayName(request.User, request.UserId?.ToString() ?? "User"), 20f);
        if (request.Achievements.Count == 0)
        {
            AddEmpty(body, "No achievements are available for this user yet.");
            return;
        }

        foreach (var achievement in request.Achievements)
        {
            var detail = achievement.Earned ? "Unlocked" : "Locked";
            if (achievement.MaxProgress > 0)
            {
                detail += " - " + achievement.Progress + "/" + achievement.MaxProgress;
            }
            AddInfoCard(body, achievement.Name, detail, achievement.Earned ? Theme.Success : Theme.TextMuted);
        }
    }

    private void BuildUserChat(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "CHAT");
        AddTitle(body, DisplayName(request.User, request.UserId?.ToString() ?? "User"), 20f);
        AddEmpty(body, "No chat transport has been received from the emulator for this user.");
    }

    private void BuildStore(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "STORE");
        AddTitle(body, request.Title ?? "Store", 20f);
        if (request.StoreItems.Count == 0)
        {
            AddEmpty(body, "No store catalog was provided by the emulator.");
            return;
        }

        foreach (var item in request.StoreItems)
        {
            AddInfoCard(body, item.Name, item.Description, Theme.Accent);
        }
    }

    private void BuildWebPage(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "WEB PAGE");
        AddTitle(body, string.IsNullOrWhiteSpace(request.Url) ? "No URL supplied" : request.Url, 16f);
        AddEmpty(body, "The game requested this overlay URL. External browser launch is disabled in the embedded overlay.");
    }

    private void BuildSettings(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "SETTINGS");
        if (request.Metadata == null || request.Metadata.Count == 0)
        {
            AddEmpty(body, "No runtime settings are available.");
            return;
        }

        AddFactGrid(body, request.Metadata.Select(kv => Tuple.Create(kv.Key, kv.Value)).ToList());
    }

    private void BuildConfirm(FlowLayoutPanel body, OverlayRequest request)
    {
        AddSectionLabel(body, "INVITATION");
        AddInvitationCard(body, request.User, request.Message ?? "You received an invitation.");

        if (request.PrimaryAction == null && request.SecondaryAction == null)
        {
            return;
        }

        var actions = new FlowLayoutPanel
        {
            Width = GetContentWidth(),
            Height = 42,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 8, 0, 0)
        };

        AddActionButton(actions, request.PrimaryActionText ?? "Confirm", Theme.Accent, request.PrimaryAction, true);
        if (request.SecondaryAction != null)
        {
            AddActionButton(actions, request.SecondaryActionText ?? "Dismiss", Theme.TextMuted, request.SecondaryAction, false);
        }
        body.Controls.Add(actions);
    }

    private void AddSummaryGrid(FlowLayoutPanel body, List<OverlaySummaryItem> items)
    {
        if (items == null || items.Count == 0)
        {
            AddEmpty(body, "No summary is available yet.");
            return;
        }

        var width = GetContentWidth();
        var columns = items.Count <= 2 ? 2 : 3;
        var rows = (items.Count + columns - 1) / columns;
        var grid = new TableLayoutPanel
        {
            Width = width,
            Height = rows * 94,
            ColumnCount = columns,
            RowCount = rows,
            Margin = new Padding(0, 2, 0, 12),
            BackColor = Theme.BgCard
        };
        for (var column = 0; column < columns; column++)
        {
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
        }
        for (var row = 0; row < rows; row++)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var card = new RoundedPanel
            {
                Radius = 10,
                FillColor = Theme.BgPanel,
                BorderColor = Theme.Border,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 10, 10)
            };

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(16, 8, 16, 8)
            };
            stack.RowStyles.Add(new RowStyle(SizeType.Percent, 60f));
            stack.RowStyles.Add(new RowStyle(SizeType.Percent, 40f));

            var valueLabel = new Label
            {
                Text = item.Value,
                ForeColor = ToneColor(item.Tone),
                Font = new Font(Theme.FontName, 21f, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = false,
                AutoEllipsis = true,
                UseMnemonic = false,
                TextAlign = ContentAlignment.BottomCenter,
                Dock = DockStyle.Fill
            };
            var label = new Label
            {
                Text = item.Label,
                ForeColor = Theme.TextMuted,
                Font = new Font(Theme.FontName, 9f, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = false,
                AutoEllipsis = true,
                UseMnemonic = false,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill
            };
            stack.Controls.Add(valueLabel, 0, 0);
            stack.Controls.Add(label, 0, 1);
            card.Controls.Add(stack);
            grid.Controls.Add(card, i % columns, i / columns);
        }

        body.Controls.Add(grid);
    }

    private void AddFactGrid(FlowLayoutPanel body, List<Tuple<string, string>> facts)
    {
        if (facts == null || facts.Count == 0)
        {
            AddEmpty(body, "No profile details are available.");
            return;
        }

        var width = GetContentWidth();
        var columns = facts.Count == 1 ? 1 : 2;
        var rows = (facts.Count + columns - 1) / columns;
        var grid = new TableLayoutPanel
        {
            Width = width,
            Height = rows * 72,
            ColumnCount = columns,
            RowCount = rows,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = Theme.BgCard
        };
        for (var column = 0; column < columns; column++)
        {
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
        }
        for (var row = 0; row < rows; row++)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
        }
        var labelWidth = Math.Max(120, width / columns - 48);

        for (int i = 0; i < facts.Count; i++)
        {
            var fact = facts[i];
            var card = new RoundedPanel
            {
                Radius = 9,
                FillColor = Theme.BgPanel,
                BorderColor = Theme.Border,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 10, 10)
            };
            card.Controls.Add(new Label
            {
                Text = fact.Item1 ?? string.Empty,
                ForeColor = Theme.Accent,
                Font = new Font(Theme.FontName, 10f, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = false,
                AutoEllipsis = true,
                UseMnemonic = false,
                Location = new Point(16, 10),
                Size = new Size(labelWidth, 20)
            });
            card.Controls.Add(new Label
            {
                Text = fact.Item2 ?? string.Empty,
                ForeColor = Theme.TextBody,
                Font = new Font(Theme.FontName, 9f, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = false,
                AutoEllipsis = true,
                UseMnemonic = false,
                Location = new Point(16, 32),
                Size = new Size(labelWidth, 20)
            });
            var column = i % columns;
            var row = i / columns;
            grid.Controls.Add(card, column, row);
            if (columns == 2 && facts.Count % 2 == 1 && i == facts.Count - 1)
            {
                grid.SetColumnSpan(card, 2);
            }
        }

        body.Controls.Add(grid);
    }

    private void AddUserRow(FlowLayoutPanel body, OverlayUser user)
    {
        var width = GetContentWidth();
        var row = new RoundedPanel
        {
            Radius = 9,
            FillColor = Theme.BgPanel,
            BorderColor = Theme.Border,
            Width = width,
            Height = 72,
            Margin = new Padding(0, 0, 0, 8)
        };

        row.Controls.Add(new AvatarControl
        {
            Bounds = new Rectangle(14, 12, 48, 48),
            DisplayName = DisplayName(user, user.SteamId.ToString()),
            AvatarPng = user.AvatarPng,
            Online = user.PersonaState != 0
        });

        row.Controls.Add(new Label
        {
            Text = DisplayName(user, user.SteamId.ToString()),
            ForeColor = Theme.TextTitle,
            Font = new Font(Theme.FontName, 11f, FontStyle.Bold, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            Location = new Point(76, 14),
            Size = new Size(Math.Max(160, width - 100), 23)
        });

        row.Controls.Add(new Label
        {
            Text = string.IsNullOrWhiteSpace(user.Status) ? (user.PersonaState != 0 ? "Online" : "Offline") : user.Status,
            ForeColor = user.PersonaState != 0 ? Theme.Success : Theme.TextMuted,
            Font = new Font(Theme.FontName, 9f, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            Location = new Point(76, 38),
            Size = new Size(Math.Max(160, width - 100), 20)
        });

        body.Controls.Add(row);
    }

    private void AddInviteUserRow(FlowLayoutPanel body, OverlayUser user, Action<OverlayUser, Action<bool>> inviteAction)
    {
        var width = GetContentWidth();
        var row = new RoundedPanel
        {
            Radius = 9,
            FillColor = Theme.BgPanel,
            BorderColor = Theme.Border,
            Width = width,
            Height = 72,
            Margin = new Padding(0, 0, 0, 8)
        };

        row.Controls.Add(new AvatarControl
        {
            Bounds = new Rectangle(14, 12, 48, 48),
            DisplayName = DisplayName(user, user.SteamId.ToString()),
            AvatarPng = user.AvatarPng,
            Online = user.PersonaState != 0
        });

        row.Controls.Add(new Label
        {
            Text = DisplayName(user, user.SteamId.ToString()),
            ForeColor = Theme.TextTitle,
            Font = new Font(Theme.FontName, 11f, FontStyle.Bold, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            Location = new Point(76, 14),
            Size = new Size(Math.Max(120, width - 220), 23)
        });

        row.Controls.Add(new Label
        {
            Text = string.IsNullOrWhiteSpace(user.Status) ? (user.PersonaState != 0 ? "Online" : "Offline") : user.Status,
            ForeColor = user.PersonaState != 0 ? Theme.Success : Theme.TextMuted,
            Font = new Font(Theme.FontName, 9f, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            Location = new Point(76, 38),
            Size = new Size(Math.Max(120, width - 220), 20)
        });

        var sending = false;
        var sent = false;
        var invite = new Button
        {
            Text = inviteAction == null ? "Unavailable" : "Invite",
            Enabled = inviteAction != null,
            Width = 102,
            Height = 32,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Theme.TextTitle,
            BackColor = Theme.BgDark,
            Font = new Font(Theme.FontName, 9f, FontStyle.Bold, GraphicsUnit.Point),
            Location = new Point(width - 118, 20),
            Cursor = inviteAction == null ? Cursors.Default : Cursors.Hand
        };
        invite.FlatAppearance.BorderColor = Theme.Accent;
        invite.FlatAppearance.MouseOverBackColor = Theme.BgCard;
        invite.Click += (sender, args) =>
        {
            if (inviteAction == null || sending || sent)
            {
                return;
            }

            sending = true;
            invite.Text = "Sending...";
            inviteAction(user, success =>
            {
                if (IsDisposed || !IsHandleCreated)
                {
                    return;
                }

                BeginInvoke((MethodInvoker)(() =>
                {
                    sending = false;
                    sent = success;
                    invite.Text = success ? "Invitation Sent" : "Retry Invite";
                    invite.ForeColor = success ? Theme.Success : Theme.TextTitle;
                    invite.FlatAppearance.BorderColor = success ? Theme.Success : Theme.Accent;
                    invite.Cursor = success ? Cursors.Default : Cursors.Hand;
                }));
            });
        };
        row.Controls.Add(invite);
        body.Controls.Add(row);
    }

    private void AddActionButton(FlowLayoutPanel actions, string text, Color color, Action action, bool primary)
    {
        if (action == null)
        {
            return;
        }

        var button = new Button
        {
            Text = text,
            Width = primary ? 140 : 110,
            Height = 34,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Theme.TextTitle,
            BackColor = Theme.BgDark,
            Font = new Font(Theme.FontName, 9f, FontStyle.Bold, GraphicsUnit.Point),
            Margin = new Padding(0, 0, 8, 0),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderColor = color;
        button.FlatAppearance.MouseOverBackColor = Theme.BgPanel;
        button.Click += (sender, args) =>
        {
            button.Enabled = false;
            action();
            HideOverlay();
        };
        actions.Controls.Add(button);
    }

    private void AddInfoCard(FlowLayoutPanel body, string title, string detail, Color accent)
    {
        var width = GetContentWidth();
        var card = new RoundedPanel
        {
            Radius = 9,
            FillColor = Theme.BgPanel,
            BorderColor = Theme.Border,
            Width = width,
            Height = 62,
            Margin = new Padding(0, 0, 0, 8)
        };
        card.Controls.Add(new Label
        {
            Text = title ?? string.Empty,
            ForeColor = accent,
            Font = new Font(Theme.FontName, 10f, FontStyle.Bold, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            Location = new Point(16, 10),
            Size = new Size(Math.Max(160, width - 40), 20)
        });
        card.Controls.Add(new Label
        {
            Text = detail ?? string.Empty,
            ForeColor = Theme.TextBody,
            Font = new Font(Theme.FontName, 9f, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            Location = new Point(16, 32),
            Size = new Size(Math.Max(160, width - 40), 20)
        });
        body.Controls.Add(card);
    }

    private void AddInvitationCard(FlowLayoutPanel body, OverlayUser user, string message)
    {
        var width = GetContentWidth();
        var card = new RoundedPanel
        {
            Radius = 9,
            FillColor = Theme.BgPanel,
            BorderColor = Theme.Border,
            Width = width,
            Height = 76,
            Margin = new Padding(0, 0, 0, 8)
        };

        card.Controls.Add(new AvatarControl
        {
            Bounds = new Rectangle(14, 14, 48, 48),
            DisplayName = DisplayName(user, "Friend"),
            AvatarPng = user?.AvatarPng,
            Online = user?.PersonaState != 0
        });
        card.Controls.Add(new Label
        {
            Text = message ?? string.Empty,
            ForeColor = Theme.TextBody,
            Font = new Font(Theme.FontName, 10f, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            Location = new Point(76, 27),
            Size = new Size(Math.Max(140, width - 96), 24)
        });
        body.Controls.Add(card);
    }

    private void AddEmpty(FlowLayoutPanel body, string text)
    {
        AddInfoCard(body, "Empty", text, Theme.TextMuted);
    }

    private void AddSectionLabel(FlowLayoutPanel body, string text)
    {
        body.Controls.Add(new Label
        {
            Text = text ?? string.Empty,
            ForeColor = Theme.TextMuted,
            Font = new Font(Theme.FontName, 8.5f, FontStyle.Bold, GraphicsUnit.Point),
            AutoSize = false,
            Width = GetContentWidth(),
            Height = 22,
            Margin = new Padding(0, 0, 0, 4)
        });
    }

    private void AddTitle(FlowLayoutPanel body, string text, float size)
    {
        body.Controls.Add(new Label
        {
            Text = text ?? string.Empty,
            ForeColor = Theme.TextTitle,
            Font = new Font(Theme.FontName, size, FontStyle.Bold, GraphicsUnit.Point),
            AutoSize = false,
            AutoEllipsis = true,
            UseMnemonic = false,
            Width = GetContentWidth(),
            Height = 42,
            Margin = new Padding(0, 0, 0, 10)
        });
    }

    private int AddChip(Control parent, string text, int x, int y, Color color)
    {
        var available = Math.Max(82, parent.ClientSize.Width - x - 24);
        var width = Math.Min(available, Math.Min(240, Math.Max(82, TextRenderer.MeasureText(text ?? string.Empty, Font).Width + 26)));
        var chip = new RoundedPanel
        {
            Radius = 12,
            FillColor = Theme.Soft(color, 36),
            BorderColor = Theme.Soft(color, 90),
            Location = new Point(x, y),
            Size = new Size(width, 26)
        };
        chip.Controls.Add(new Label
        {
            Text = text ?? string.Empty,
            ForeColor = color,
            Font = new Font(Theme.FontName, 8.5f, FontStyle.Bold, GraphicsUnit.Point),
            TextAlign = ContentAlignment.MiddleCenter,
            AutoEllipsis = true,
            UseMnemonic = false,
            Dock = DockStyle.Fill
        });
        parent.Controls.Add(chip);
        return width;
    }

    private FlowLayoutPanel CreateScrollBody()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = false,
            BackColor = Theme.BgCard,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };
    }

    private void UpdateScrollMode(Control page)
    {
        if (page is not FlowLayoutPanel body)
        {
            return;
        }

        body.AutoScroll = false;
        body.PerformLayout();

        var contentBottom = 0;
        foreach (Control child in body.Controls)
        {
            if (!child.Visible)
            {
                continue;
            }

            contentBottom = Math.Max(contentBottom, child.Bottom + child.Margin.Bottom);
        }

        body.AutoScroll = contentBottom > body.ClientSize.Height + 2;
        body.PerformLayout();
    }

    private int GetContentWidth()
    {
        var width = _contentHost.ClientSize.Width - _contentHost.Padding.Horizontal - 4;
        if (width <= 0)
        {
            width = PreferredContentWidth;
        }

        return Math.Max(320, Math.Min(PreferredContentWidth, width));
    }

    private void PositionOverTarget()
    {
        var bounds = ResolveTargetBounds();
        Bounds = bounds;
        var maxWidth = Math.Max(360, bounds.Width - (_options.Margin * 2));
        var maxHeight = Math.Max(360, bounds.Height - (_options.Margin * 2));
        _card.Size = new Size(
            Math.Min(PreferredCardWidth, maxWidth),
            Math.Min(PreferredCardHeight, maxHeight));
        _card.Location = new Point(
            Math.Max(0, (ClientSize.Width - _card.Width) / 2),
            Math.Max(0, (ClientSize.Height - _card.Height) / 2));
    }

    private Rectangle ResolveTargetBounds()
    {
        if (_options.TargetWindowHandle != IntPtr.Zero &&
            NativeMethods.IsWindow(_options.TargetWindowHandle) &&
            !NativeMethods.IsIconic(_options.TargetWindowHandle) &&
            NativeMethods.GetWindowRect(_options.TargetWindowHandle, out var rect))
        {
            var width = Math.Max(1, rect.Right - rect.Left);
            var height = Math.Max(1, rect.Bottom - rect.Top);
            return new Rectangle(rect.Left, rect.Top, width, height);
        }

        return Screen.PrimaryScreen.Bounds;
    }

    private static string ResolveWindowTitle(OverlayRequest request)
    {
        return request.Kind switch
        {
            OverlayKind.People => "People",
            OverlayKind.Invite => "Invite Friends",
            OverlayKind.UserChat => "Chat",
            OverlayKind.UserStats => "Stats",
            OverlayKind.UserAchievements => "Achievements",
            OverlayKind.Store => request.Title ?? "Store",
            OverlayKind.WebPage => "Web Page",
            OverlayKind.Settings => "Settings",
            OverlayKind.ConfirmAction => request.Title ?? "Confirm Action",
            OverlayKind.UserProfile => request.Title ?? "User Profile",
            _ => "Home"
        };
    }

    private static string DisplayName(OverlayUser user, string fallback)
    {
        if (user != null && !string.IsNullOrWhiteSpace(user.PersonaName))
        {
            return user.PersonaName;
        }

        return string.IsNullOrWhiteSpace(fallback) ? "User" : fallback;
    }

    private static string RelationshipLabel(OverlayUser user)
    {
        if (user == null)
        {
            return "Unknown";
        }

        if (user.IsSelf)
        {
            return "Current user";
        }

        if (user.HasFriend || user.FriendRelationship == 3)
        {
            return "Friend";
        }

        switch (user.FriendRelationship)
        {
            case 2:
                return "Incoming request";
            case 4:
                return "Outgoing request";
            case 1:
            case 5:
            case 6:
                return "Blocked";
            default:
                return "Not friends";
        }
    }

    private static string CompactStatus(string status, ulong lobbyId)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return "Offline";
        }

        var value = status.Trim();
        if (value.StartsWith("In game", StringComparison.OrdinalIgnoreCase))
        {
            return "In game";
        }

        if (lobbyId != 0)
        {
            value = value.Replace(lobbyId.ToString(), string.Empty).Trim(' ', '-', ':');
        }

        return value.Length <= 28 ? value : value.Substring(0, 28);
    }

    private static Dictionary<string, string> FilterProfileRichPresence(OverlayUser user)
    {
        var result = new Dictionary<string, string>();
        if (user?.RichPresence == null || user.RichPresence.Count == 0)
        {
            return result;
        }

        var lobby = user.LobbyId == 0 ? string.Empty : user.LobbyId.ToString();
        foreach (var item in user.RichPresence)
        {
            if (string.IsNullOrWhiteSpace(item.Key) || string.IsNullOrWhiteSpace(item.Value))
            {
                continue;
            }

            if (string.Equals(item.Key, "lobby", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.Value, lobby, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            result[item.Key] = item.Value;
        }

        return result;
    }

    private static Color ToneColor(string tone)
    {
        switch ((tone ?? string.Empty).ToLowerInvariant())
        {
            case "success":
                return Theme.Success;
            case "warning":
                return Theme.Warning;
            case "error":
                return Theme.Error;
            default:
                return Theme.Accent;
        }
    }

    private sealed class RoundedPanel : Panel
    {
        public int Radius { get; set; } = 8;
        public Color FillColor { get; set; } = Theme.BgPanel;
        public Color BorderColor { get; set; } = Theme.Border;

        public RoundedPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = RoundedRect(ClientRectangle, Radius))
            using (var fill = new SolidBrush(FillColor))
            using (var pen = new Pen(BorderColor))
            {
                e.Graphics.FillPath(fill, path);
                var rect = ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                using (var borderPath = RoundedRect(rect, Radius))
                {
                    e.Graphics.DrawPath(pen, borderPath);
                }
            }
        }
    }

    private sealed class AvatarControl : Control
    {
        public string DisplayName { get; set; } = string.Empty;
        public byte[] AvatarPng { get; set; } = new byte[0];
        public bool Online { get; set; }

        public AvatarControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var background = new SolidBrush(Theme.BgPanel))
            {
                e.Graphics.FillRectangle(background, ClientRectangle);
            }

            var rect = ClientRectangle;
            rect.Inflate(-1, -1);

            using (var path = RoundedRect(rect, rect.Width / 2))
            {
                e.Graphics.SetClip(path);
                using (var image = TryLoadImage(AvatarPng))
                {
                    if (image != null)
                    {
                        e.Graphics.DrawImage(image, rect);
                    }
                    else
                    {
                        using (var brush = new LinearGradientBrush(rect, Theme.Accent, Theme.BgDark, 45f))
                        {
                            e.Graphics.FillEllipse(brush, rect);
                        }
                        var initials = BuildInitials(DisplayName);
                        using (var font = new Font(Theme.FontName, Math.Max(12f, rect.Width * 0.28f), FontStyle.Bold, GraphicsUnit.Point))
                        using (var text = new SolidBrush(Theme.TextTitle))
                        using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            e.Graphics.DrawString(initials, font, text, rect, format);
                        }
                    }
                }
                e.Graphics.ResetClip();
            }

            using (var pen = new Pen(Online ? Theme.Success : Theme.BorderSoft, 3f))
            {
                e.Graphics.DrawEllipse(pen, rect);
            }
        }

        private static Image TryLoadImage(byte[] png)
        {
            if (png == null || png.Length == 0)
            {
                return null;
            }

            try
            {
                using (var ms = new MemoryStream(png))
                using (var loaded = Image.FromStream(ms))
                {
                    return new Bitmap(loaded);
                }
            }
            catch
            {
                return null;
            }
        }

        private static string BuildInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "?";
            }

            var parts = name.Trim().Split(new[] { ' ', '\t', '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return parts[0].Substring(0, 1).ToUpperInvariant();
            }

            return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpperInvariant();
        }
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        var diameter = Math.Max(1, radius * 2);
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}
