using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFinder.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixChatMessageEventCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Events_EventId",
                table: "ChatMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Events_EventId",
                table: "ChatMessages",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Events_EventId",
                table: "ChatMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Events_EventId",
                table: "ChatMessages",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
