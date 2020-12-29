using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    IsSuperUser = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleToRolePermission",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PermissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleToRolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RoleToRolePermission_RolePermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "RolePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleToRolePermission_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoggingData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Distance = table.Column<int>(nullable: false),
                    Time = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoggingData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoggingData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "ManageUsers" },
                    { 2, "ManageOwnJoggingData" },
                    { 3, "ManageOthersJoggingData" }
                });

            migrationBuilder.InsertData(
                table: "ServerSettings",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 1, "DefaultUserRole", "1" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "IsSuperUser", "Name" },
                values: new object[,]
                {
                    { 1, false, "User" },
                    { 2, false, "Manager" },
                    { 3, true, "Administrator" }
                });

            migrationBuilder.InsertData(
                table: "RoleToRolePermission",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 2, 2 },
                    { 2, 1 },
                    { 3, 2 },
                    { 3, 3 },
                    { 3, 1 }
                });

            migrationBuilder.InsertData(
                "Users",
                new[] {"Id", "UserName", "Email", "Password", "Salt", "RoleId"},
                new object[]
                {
                    1, "Administrator", "admin@local", "nV+l2cMZtrY0Q/+FajqQ1t6Go0BcqROmtSvfUOk6fB0=",
                    "LXMojCywDK3n6oPH8PaUkg==", 3
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoggingData_UserId",
                table: "JoggingData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleToRolePermission_PermissionId",
                table: "RoleToRolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoggingData");

            migrationBuilder.DropTable(
                name: "RoleToRolePermission");

            migrationBuilder.DropTable(
                name: "ServerSettings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
