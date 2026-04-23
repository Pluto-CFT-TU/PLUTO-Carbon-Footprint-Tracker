using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PLUTO_Carbon_Footprint_Tracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Auth0UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Membership = table.Column<string>(type: "TEXT", nullable: true),
                    Rank = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: false),
                    EmissionFactor = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    VideoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    IsFaq = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tips_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailNotificationsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SmsNotificationsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    PushNotificationsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Muted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    HighContrast = table.Column<bool>(type: "INTEGER", nullable: false),
                    TextToSpeech = table.Column<bool>(type: "INTEGER", nullable: false),
                    ColourBlindMode = table.Column<string>(type: "TEXT", nullable: true),
                    AllowAnalytics = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowMarketingEmails = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmissionEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActivityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    CarbonKg = table.Column<decimal>(type: "TEXT", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmissionEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmissionEntries_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmissionEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ActivityTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Transport" },
                    { 2, null, "Food" },
                    { 3, null, "Energy" },
                    { 4, null, "Leisure" }
                });

            migrationBuilder.InsertData(
                table: "NewsArticles",
                columns: new[] { "Id", "Category", "Content", "ImageUrl", "IsPublished", "PublishedAt", "Summary", "Title" },
                values: new object[,]
                {
                    { 1, "Climate", "Experts warn that immediate global action is required to prevent irreversible climate damage. Policy changes and individual responsibility are key.", "/Images/News/Actnow.jpg", true, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Governments are being urged to take immediate action to reduce emissions.", "Act Now: Climate Action Urged Globally" },
                    { 2, "Environment", "Deforestation remains one of the largest contributors to carbon emissions. Conservation efforts are critical to reversing the damage.", "/Images/News/Deforest.jpg", true, new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Forests are being cleared at alarming rates, increasing global CO₂ levels.", "Deforestation Rates Continue to Rise" },
                    { 3, "Climate", "Rising global temperatures have led to more frequent wildfires, significantly impacting carbon emissions and air quality.", "/Images/News/Fire.jpg", true, new DateTime(2026, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Increasing wildfires are releasing massive amounts of CO₂.", "Wildfires Contribute to Record Emissions" },
                    { 4, "Energy", "Countries are investing heavily in wind energy infrastructure to reduce reliance on fossil fuels.", "/Images/News/grassywindmills.jpg", true, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wind farms are becoming more common as renewable energy demand grows.", "Wind Energy Expands Across Rural Areas" },
                    { 5, "Climate", "Scientists report that ice caps are melting faster than expected, posing a serious threat to coastal regions.", "/Images/News/ice.jpg", true, new DateTime(2026, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Polar ice melt is accelerating, raising sea levels globally.", "Melting Ice Caps Signal Climate Emergency" },
                    { 6, "Awareness", "Without immediate changes, future generations could face severe environmental challenges including extreme weather and resource shortages.", "/Images/News/Nofuture.jpg", true, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Experts outline the consequences of failing to reduce emissions.", "A Future Without Action Looks Grim" },
                    { 7, "Energy", "Urban centres transitioning to renewable energy sources are experiencing measurable decreases in carbon output.", "/Images/News/grassywindmills.jpg", true, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cities adopting renewables are seeing emission reductions.", "Renewables Help Cut Urban Emissions" },
                    { 8, "Awareness", "Educational campaigns are helping people understand how daily habits impact their carbon footprint.", "/Images/News/Actnow.jpg", true, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Public awareness is increasing around sustainability.", "Climate Awareness Campaigns Gain Momentum" }
                });

            migrationBuilder.InsertData(
                table: "Tips",
                columns: new[] { "Id", "ActivityTypeId", "Content", "IsFaq", "Title", "VideoUrl" },
                values: new object[,]
                {
                    { 13, null, "Small daily changes can add up to a significant reduction over time.", false, "Start Small", "https://www.youtube.com/embed/BNzgS95r8U8" },
                    { 14, null, "Regularly reviewing your emissions helps you stay motivated.", false, "Track Your Progress", null },
                    { 15, null, "Your emissions are calculated using predefined emission factors multiplied by your activity quantity.", true, "How is my carbon footprint calculated?", null },
                    { 16, null, "Yes. You can edit or delete entries from the Emissions Log page.", true, "Can I edit my entries?", null },
                    { 17, null, "Recommendations are based on the activity type contributing the most CO2 in your logs.", true, "Why do I see recommended tips?", null },
                    { 18, null, "No, but regular logging improves the accuracy of insights and recommendations.", true, "Do I need to log every day?", null },
                    { 19, null, "Your account uses Auth0 authentication and your data is stored securely.", true, "Is my data secure?", null }
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Id", "ActivityTypeId", "EmissionFactor", "IsActive", "Name", "Unit" },
                values: new object[,]
                {
                    { 1, 1, 0.411m, true, "Car Travel", "miles" },
                    { 2, 1, 0.105m, true, "Bus Travel", "miles" },
                    { 3, 1, 0.041m, true, "Train Travel", "miles" },
                    { 4, 1, 0.255m, true, "Flight", "miles" },
                    { 5, 2, 2.500m, true, "Beef Meal", "meals" },
                    { 6, 2, 1.100m, true, "Chicken Meal", "meals" },
                    { 7, 2, 0.600m, true, "Vegetarian Meal", "meals" },
                    { 8, 2, 0.300m, true, "Dairy Consumption", "servings" },
                    { 9, 3, 0.050m, true, "Phone Charging", "hours" },
                    { 10, 3, 0.080m, true, "Laptop Use", "hours" },
                    { 11, 3, 0.300m, true, "Heating Use", "hours" },
                    { 12, 3, 0.600m, true, "Washing Machine", "cycles" },
                    { 13, 4, 0.036m, true, "Streaming TV", "hours" },
                    { 14, 4, 0.120m, true, "Gaming Console", "hours" },
                    { 15, 4, 1.800m, true, "Cinema Trip", "trips" },
                    { 16, 4, 0.411m, true, "Gym Travel", "miles" }
                });

            migrationBuilder.InsertData(
                table: "Tips",
                columns: new[] { "Id", "ActivityTypeId", "Content", "IsFaq", "Title", "VideoUrl" },
                values: new object[,]
                {
                    { 1, 1, "Try walking, cycling, or public transport for shorter journeys to reduce travel emissions.", false, "Reduce Car Journeys", null },
                    { 2, 1, "Buses and trains produce significantly less CO2 per passenger than cars.", false, "Use Public Transport", null },
                    { 3, 1, "Plan routes efficiently to reduce total distance travelled.", false, "Combine Trips", null },
                    { 4, 2, "Reducing beef consumption can significantly lower your carbon footprint.", false, "Eat Less Red Meat", null },
                    { 5, 2, "Even one meat-free day a week can make a difference.", false, "Try Meat-Free Days", null },
                    { 6, 2, "Plan meals and store food properly to avoid unnecessary waste.", false, "Reduce Food Waste", null },
                    { 7, 3, "Electronics still use power in standby mode—switch them off completely.", false, "Turn Off Standby Devices", null },
                    { 8, 3, "Reducing your heating by even 1°C can cut energy consumption.", false, "Lower Heating Usage", null },
                    { 9, 3, "Modern appliances use significantly less energy than older ones.", false, "Use Energy Efficient Appliances", null },
                    { 10, 4, "Lowering video quality reduces data usage and energy consumption.", false, "Reduce Streaming Quality", null },
                    { 11, 4, "Gaming consoles can consume significant power during long sessions.", false, "Limit Gaming Time", null },
                    { 12, 4, "Outdoor activities have a much lower environmental impact.", false, "Choose Outdoor Activities", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivityTypeId",
                table: "Activities",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmissionEntries_ActivityId",
                table: "EmissionEntries",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_EmissionEntries_UserId",
                table: "EmissionEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_UserId",
                table: "NotificationSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tips_ActivityTypeId",
                table: "Tips",
                column: "ActivityTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmissionEntries");

            migrationBuilder.DropTable(
                name: "NewsArticles");

            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Tips");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ActivityTypes");
        }
    }
}
