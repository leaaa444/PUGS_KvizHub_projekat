using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvizHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDisconnectedAtToParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DisconnectedAt",
                table: "LiveQuizParticipants",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisconnectedAt",
                table: "LiveQuizParticipants");
        }
    }
}
