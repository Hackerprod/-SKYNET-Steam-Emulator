using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKYNET_server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AppStateAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SteamId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "SteamId",
                table: "Lobbies",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "SteamId",
                table: "GameServers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "LobbyId",
                table: "DotaMatches",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "SteamId",
                table: "Avatars",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "AppState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActiveWebSteamId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppState", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebSessions_SteamId",
                table: "WebSessions",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_WebAccounts_SteamId",
                table: "WebAccounts",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_FromSteamId",
                table: "FriendRequests",
                column: "FromSteamId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ToSteamId",
                table: "FriendRequests",
                column: "ToSteamId");

            migrationBuilder.CreateIndex(
                name: "IX_DotaMatchPlayers_SteamId",
                table: "DotaMatchPlayers",
                column: "SteamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppState");

            migrationBuilder.DropIndex(
                name: "IX_WebSessions_SteamId",
                table: "WebSessions");

            migrationBuilder.DropIndex(
                name: "IX_WebAccounts_SteamId",
                table: "WebAccounts");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_FromSteamId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_ToSteamId",
                table: "FriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_DotaMatchPlayers_SteamId",
                table: "DotaMatchPlayers");

            migrationBuilder.AlterColumn<long>(
                name: "SteamId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "SteamId",
                table: "Lobbies",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "SteamId",
                table: "GameServers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "LobbyId",
                table: "DotaMatches",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<long>(
                name: "SteamId",
                table: "Avatars",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
