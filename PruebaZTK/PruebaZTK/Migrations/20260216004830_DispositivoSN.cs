using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaZTK.Migrations
{
    /// <inheritdoc />
    public partial class DispositivoSN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DispositivoSN",
                table: "Asistencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispositivoSN",
                table: "Asistencias");
        }
    }
}
