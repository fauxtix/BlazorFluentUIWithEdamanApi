﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdamanFluentApi.Migrations.Youtube
{
    /// <inheritdoc />
    public partial class fourth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryTypes_Categorias_CategoriaId",
                table: "CategoryTypes");

            migrationBuilder.DropIndex(
                name: "IX_CategoryTypes_CategoriaId",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "TipoCategoria",
                table: "Categorias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "CategoryTypes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoCategoria",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTypes_CategoriaId",
                table: "CategoryTypes",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryTypes_Categorias_CategoriaId",
                table: "CategoryTypes",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }
    }
}
