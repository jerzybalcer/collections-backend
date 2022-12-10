using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Collections.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TagsValuesInContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagValue_Items_ItemId",
                table: "TagValue");

            migrationBuilder.DropForeignKey(
                name: "FK_TagValue_Tags_TagId",
                table: "TagValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagValue",
                table: "TagValue");

            migrationBuilder.RenameTable(
                name: "TagValue",
                newName: "TagsValues");

            migrationBuilder.RenameIndex(
                name: "IX_TagValue_TagId",
                table: "TagsValues",
                newName: "IX_TagsValues_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_TagValue_ItemId",
                table: "TagsValues",
                newName: "IX_TagsValues_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagsValues",
                table: "TagsValues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagsValues_Items_ItemId",
                table: "TagsValues",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagsValues_Tags_TagId",
                table: "TagsValues",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagsValues_Items_ItemId",
                table: "TagsValues");

            migrationBuilder.DropForeignKey(
                name: "FK_TagsValues_Tags_TagId",
                table: "TagsValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagsValues",
                table: "TagsValues");

            migrationBuilder.RenameTable(
                name: "TagsValues",
                newName: "TagValue");

            migrationBuilder.RenameIndex(
                name: "IX_TagsValues_TagId",
                table: "TagValue",
                newName: "IX_TagValue_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_TagsValues_ItemId",
                table: "TagValue",
                newName: "IX_TagValue_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagValue",
                table: "TagValue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagValue_Items_ItemId",
                table: "TagValue",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagValue_Tags_TagId",
                table: "TagValue",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
