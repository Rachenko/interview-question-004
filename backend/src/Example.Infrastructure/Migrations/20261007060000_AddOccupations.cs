using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Example.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOccupations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "occupations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_occupations", x => x.Id);
                });

            migrationBuilder.Sql(@"
INSERT INTO occupations (""Id"", ""Name"") VALUES
    (1, 'Software Developer'),
    (2, 'System Analyst'),
    (3, 'Project Manager'),
    (4, 'QA Engineer'),
    (5, 'UX/UI Designer'),
    (6, 'Business Analyst'),
    (7, 'DevOps Engineer'),
    (8, 'Data Engineer');

SELECT setval(pg_get_serial_sequence('occupations', 'Id'), 8);
");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "persons");

            migrationBuilder.AddColumn<int>(
                name: "OccupationId",
                table: "persons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_persons_OccupationId",
                table: "persons",
                column: "OccupationId");

            migrationBuilder.AddForeignKey(
                name: "FK_persons_occupations_OccupationId",
                table: "persons",
                column: "OccupationId",
                principalTable: "occupations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_persons_occupations_OccupationId",
                table: "persons");

            migrationBuilder.DropIndex(
                name: "IX_persons_OccupationId",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "OccupationId",
                table: "persons");

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropTable(
                name: "occupations");
        }
    }
}
