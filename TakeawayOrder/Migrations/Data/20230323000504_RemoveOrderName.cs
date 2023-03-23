using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TakeawayOrder.Migrations.Data
{
    /// <inheritdoc />
    public partial class RemoveOrderName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderName",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
