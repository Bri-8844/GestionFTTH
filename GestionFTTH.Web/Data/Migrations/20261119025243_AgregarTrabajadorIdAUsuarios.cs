using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionFTTH.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTrabajadorIdAUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrabajadorId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrabajadorId",
                table: "AspNetUsers");
        }
    }
}
