using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKYNET_server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RedesignRemoteStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE TempRemoteFiles (
                    Key TEXT,
                    FileName TEXT,
                    Content BLOB,
                    Size INTEGER,
                    Timestamp INTEGER
                );
            ");

            migrationBuilder.Sql(@"
                INSERT INTO TempRemoteFiles SELECT Key, FileName, Content, Size, Timestamp FROM RemoteFiles;
            ");

            migrationBuilder.Sql(@"
                DROP TABLE RemoteFiles;
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE RemoteFiles (
                    OwnerSteamId INTEGER NOT NULL,
                    AppId INTEGER NOT NULL,
                    NormalizedName TEXT NOT NULL,
                    OriginalName TEXT NOT NULL,
                    Content BLOB NOT NULL,
                    Size INTEGER NOT NULL,
                    Timestamp INTEGER NOT NULL,
                    Sha256 TEXT NOT NULL,
                    SyncPlatforms INTEGER NOT NULL,
                    Version INTEGER NOT NULL,
                    Persisted INTEGER NOT NULL,
                    DeletedAt TEXT,
                    PRIMARY KEY (OwnerSteamId, AppId, NormalizedName)
                );
            ");

            migrationBuilder.Sql(@"
                INSERT INTO RemoteFiles (OwnerSteamId, AppId, NormalizedName, OriginalName, Content, Size, Timestamp, Sha256, SyncPlatforms, Version, Persisted, DeletedAt)
                SELECT
                    COALESCE((SELECT SteamId FROM Users LIMIT 1), 0) * (CASE WHEN (SELECT COUNT(*) FROM Users) = 1 THEN 1 ELSE 0 END) as OwnerSteamId,
                    COALESCE((SELECT AppId FROM Users LIMIT 1), 0) * (CASE WHEN (SELECT COUNT(*) FROM Users) = 1 THEN 1 ELSE 0 END) as AppId,
                    LOWER(REPLACE(COALESCE(Key, FileName), '\', '/')) as NormalizedName,
                    COALESCE(FileName, Key) as OriginalName,
                    COALESCE(Content, X''),
                    COALESCE(Size, 0),
                    COALESCE(Timestamp, 0),
                    '',
                    -1,
                    1,
                    1,
                    NULL
                FROM TempRemoteFiles;
            ");

            migrationBuilder.Sql(@"
                DROP TABLE TempRemoteFiles;
            ");

            migrationBuilder.CreateTable(
                name: "RemoteFileShares",
                columns: table => new
                {
                    Handle = table.Column<long>(type: "INTEGER", nullable: false),
                    OwnerSteamId = table.Column<long>(type: "INTEGER", nullable: false),
                    AppId = table.Column<uint>(type: "INTEGER", nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoteFileShares", x => x.Handle);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemoteFileShares");

            migrationBuilder.Sql(@"
                DROP TABLE RemoteFiles;
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE RemoteFiles (
                    Key TEXT PRIMARY KEY,
                    FileName TEXT,
                    Content BLOB,
                    Size INTEGER,
                    Timestamp INTEGER
                );
            ");
        }
    }
}
