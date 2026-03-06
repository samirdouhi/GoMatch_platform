using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateNaissance = table.Column<DateOnly>(type: "date", nullable: true),
                    Genre = table.Column<int>(type: "int", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Langue = table.Column<int>(type: "int", nullable: false),
                    InscriptionTerminee = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ProfileType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Departement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CommerceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Nationalite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Preferences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BudgetRange = table.Column<int>(type: "int", nullable: true),
                    DureeMoyenne = table.Column<int>(type: "int", nullable: true),
                    TypeTouriste = table.Column<int>(type: "int", nullable: true),
                    EquipesSuivies = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Departement",
                table: "Profiles",
                column: "Departement");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
