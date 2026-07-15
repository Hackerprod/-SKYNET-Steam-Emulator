using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKYNET_server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Earned = table.Column<bool>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Progress = table.Column<uint>(type: "INTEGER", nullable: false),
                    MaxProgress = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => new { x.SteamId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Avatars",
                columns: table => new
                {
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avatars", x => x.SteamId);
                });

            migrationBuilder.CreateTable(
                name: "CosmeticSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DotaPath = table.Column<string>(type: "TEXT", nullable: false),
                    LastImportAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastImportStatus = table.Column<string>(type: "TEXT", nullable: false),
                    EquipmentVersion = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosmeticSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DotaEquipment",
                columns: table => new
                {
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    HeroId = table.Column<uint>(type: "INTEGER", nullable: false),
                    SlotId = table.Column<uint>(type: "INTEGER", nullable: false),
                    HeroName = table.Column<string>(type: "TEXT", nullable: false),
                    Slot = table.Column<string>(type: "TEXT", nullable: false),
                    DefIndex = table.Column<uint>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<long>(type: "INTEGER", nullable: false),
                    Style = table.Column<uint>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotaEquipment", x => new { x.SteamId, x.HeroId, x.SlotId });
                });

            migrationBuilder.CreateTable(
                name: "DotaHeroIds",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    HeroId = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotaHeroIds", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "DotaHeroSlots",
                columns: table => new
                {
                    HeroName = table.Column<string>(type: "TEXT", nullable: false),
                    SlotName = table.Column<string>(type: "TEXT", nullable: false),
                    SlotId = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotaHeroSlots", x => new { x.HeroName, x.SlotName });
                });

            migrationBuilder.CreateTable(
                name: "DotaItems",
                columns: table => new
                {
                    DefIndex = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Prefab = table.Column<string>(type: "TEXT", nullable: false),
                    Slot = table.Column<string>(type: "TEXT", nullable: false),
                    Quality = table.Column<string>(type: "TEXT", nullable: false),
                    QualityId = table.Column<uint>(type: "INTEGER", nullable: false),
                    Rarity = table.Column<string>(type: "TEXT", nullable: false),
                    RarityId = table.Column<uint>(type: "INTEGER", nullable: false),
                    ImageInventory = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTool = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBundle = table.Column<bool>(type: "INTEGER", nullable: false),
                    HeroNames = table.Column<string>(type: "TEXT", nullable: false),
                    HeroIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotaItems", x => x.DefIndex);
                });

            migrationBuilder.CreateTable(
                name: "DotaMatches",
                columns: table => new
                {
                    LobbyId = table.Column<long>(type: "INTEGER", nullable: false),
                    MatchId = table.Column<long>(type: "INTEGER", nullable: false),
                    ServerSteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    Connect = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<uint>(type: "INTEGER", nullable: false),
                    GameState = table.Column<uint>(type: "INTEGER", nullable: false),
                    GameStartTime = table.Column<uint>(type: "INTEGER", nullable: false),
                    Dedicated = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotaMatches", x => x.LobbyId);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FromSteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    ToSteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    FriendSteamId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => new { x.SteamId, x.FriendSteamId });
                });

            migrationBuilder.CreateTable(
                name: "GameServers",
                columns: table => new
                {
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    AppId = table.Column<uint>(type: "INTEGER", nullable: false),
                    IP = table.Column<uint>(type: "INTEGER", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    QueryPort = table.Column<int>(type: "INTEGER", nullable: false),
                    Flags = table.Column<uint>(type: "INTEGER", nullable: false),
                    Secure = table.Column<byte>(type: "INTEGER", nullable: false),
                    VersionString = table.Column<string>(type: "TEXT", nullable: false),
                    Product = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ModDir = table.Column<string>(type: "TEXT", nullable: false),
                    Dedicated = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    BotPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerName = table.Column<string>(type: "TEXT", nullable: false),
                    MapName = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordProtected = table.Column<bool>(type: "INTEGER", nullable: false),
                    SpectatorPort = table.Column<uint>(type: "INTEGER", nullable: false),
                    SpectatorServerName = table.Column<string>(type: "TEXT", nullable: false),
                    GameTags = table.Column<string>(type: "TEXT", nullable: false),
                    GameData = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    KeyValues = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServers", x => x.SteamId);
                });

            migrationBuilder.CreateTable(
                name: "Lobbies",
                columns: table => new
                {
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    AppId = table.Column<uint>(type: "INTEGER", nullable: false),
                    OwnerSteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    LobbyType = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxMembers = table.Column<int>(type: "INTEGER", nullable: false),
                    Joinable = table.Column<bool>(type: "INTEGER", nullable: false),
                    LobbyData = table.Column<string>(type: "TEXT", nullable: false),
                    Members = table.Column<string>(type: "TEXT", nullable: false),
                    GameServer = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobbies", x => x.SteamId);
                });

            migrationBuilder.CreateTable(
                name: "RemoteFiles",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoteFiles", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => new { x.SteamId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<uint>(type: "INTEGER", nullable: false),
                    PersonaName = table.Column<string>(type: "TEXT", nullable: false),
                    AppId = table.Column<uint>(type: "INTEGER", nullable: false),
                    PlayerLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.SteamId);
                });

            migrationBuilder.CreateTable(
                name: "WebAccounts",
                columns: table => new
                {
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebAccounts", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "WebSessions",
                columns: table => new
                {
                    AccessToken = table.Column<string>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: false),
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    ClientInstanceId = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessRole = table.Column<string>(type: "TEXT", nullable: false),
                    RemoteIp = table.Column<string>(type: "TEXT", nullable: true),
                    LastSeenUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WebSession = table.Column<bool>(type: "INTEGER", nullable: false),
                    Persistent = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSessions", x => x.AccessToken);
                });

            migrationBuilder.CreateTable(
                name: "DotaMatchPlayers",
                columns: table => new
                {
                    LobbyId = table.Column<long>(type: "INTEGER", nullable: false),
                    SteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<uint>(type: "INTEGER", nullable: false),
                    PersonaName = table.Column<string>(type: "TEXT", nullable: false),
                    Team = table.Column<uint>(type: "INTEGER", nullable: false),
                    Slot = table.Column<uint>(type: "INTEGER", nullable: false),
                    CoachTeam = table.Column<uint>(type: "INTEGER", nullable: false),
                    HeroId = table.Column<uint>(type: "INTEGER", nullable: false),
                    Equipment = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotaMatchPlayers", x => new { x.LobbyId, x.SteamId });
                    table.ForeignKey(
                        name: "FK_DotaMatchPlayers_DotaMatches_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "DotaMatches",
                        principalColumn: "LobbyId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Avatars");

            migrationBuilder.DropTable(
                name: "CosmeticSettings");

            migrationBuilder.DropTable(
                name: "DotaEquipment");

            migrationBuilder.DropTable(
                name: "DotaHeroIds");

            migrationBuilder.DropTable(
                name: "DotaHeroSlots");

            migrationBuilder.DropTable(
                name: "DotaItems");

            migrationBuilder.DropTable(
                name: "DotaMatchPlayers");

            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "GameServers");

            migrationBuilder.DropTable(
                name: "Lobbies");

            migrationBuilder.DropTable(
                name: "RemoteFiles");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WebAccounts");

            migrationBuilder.DropTable(
                name: "WebSessions");

            migrationBuilder.DropTable(
                name: "DotaMatches");
        }
    }
}
