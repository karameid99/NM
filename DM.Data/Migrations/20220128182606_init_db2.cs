using Microsoft.EntityFrameworkCore.Migrations;

namespace NM.Data.Migrations
{
    public partial class init_db2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductHistories_ShelfProducts_OldShelfProductId",
                table: "ProductHistories");

            migrationBuilder.AlterColumn<int>(
                name: "OldShelfProductId",
                table: "ProductHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductHistories_ShelfProducts_OldShelfProductId",
                table: "ProductHistories",
                column: "OldShelfProductId",
                principalTable: "ShelfProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductHistories_ShelfProducts_OldShelfProductId",
                table: "ProductHistories");

            migrationBuilder.AlterColumn<int>(
                name: "OldShelfProductId",
                table: "ProductHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductHistories_ShelfProducts_OldShelfProductId",
                table: "ProductHistories",
                column: "OldShelfProductId",
                principalTable: "ShelfProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
