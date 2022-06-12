using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OWArcadeBackend.Persistence.Migrations
{
    public partial class initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArcadeModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Game = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Players = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArcadeModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contributors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "default.jpg"),
                    Group = table.Column<int>(type: "int", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Profile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Whitelist",
                columns: table => new
                {
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Whitelist", x => x.ProviderKey);
                });

            migrationBuilder.CreateTable(
                name: "Dailies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Game = table.Column<int>(type: "int", nullable: false),
                    ContributorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dailies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dailies_Contributors_ContributorId",
                        column: x => x.ContributorId,
                        principalTable: "Contributors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TileMode",
                columns: table => new
                {
                    DailyId = table.Column<int>(type: "int", nullable: false),
                    TileId = table.Column<int>(type: "int", nullable: false),
                    ArcadeModeId = table.Column<int>(type: "int", nullable: false),
                    LabelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TileMode", x => new { x.DailyId, x.TileId });
                    table.ForeignKey(
                        name: "FK_TileMode_ArcadeModes_ArcadeModeId",
                        column: x => x.ArcadeModeId,
                        principalTable: "ArcadeModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TileMode_Dailies_DailyId",
                        column: x => x.DailyId,
                        principalTable: "Dailies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TileMode_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ArcadeModes",
                columns: new[] { "Id", "Description", "Game", "Image", "Name", "Players" },
                values: new object[,]
                {
                    { 1, "Placeholder", 0, "DA5533B1DF3F2DBF78F47A71B115BE43.jpg", "OverwatchArcade.Today", "1v1" },
                    { 2, "Placeholder", 1, "DA5533B1DF3F2DBF78F47A71B115BE43.jpg", "OverwatchArcade.Today", "1v1" }
                });

            migrationBuilder.InsertData(
                table: "Config",
                columns: new[] { "Id", "JsonValue", "Key", "Value" },
                values: new object[,]
                {
                    { 1, "[{\"name\":\"Afghanistan\",\"code\":\"AF\"},{\"name\":\"land Islands\",\"code\":\"AX\"},{\"name\":\"Albania\",\"code\":\"AL\"},{\"name\":\"Algeria\",\"code\":\"DZ\"},{\"name\":\"American Samoa\",\"code\":\"AS\"},{\"name\":\"AndorrA\",\"code\":\"AD\"},{\"name\":\"Angola\",\"code\":\"AO\"},{\"name\":\"Anguilla\",\"code\":\"AI\"},{\"name\":\"Antarctica\",\"code\":\"AQ\"},{\"name\":\"Antigua and Barbuda\",\"code\":\"AG\"},{\"name\":\"Argentina\",\"code\":\"AR\"},{\"name\":\"Armenia\",\"code\":\"AM\"},{\"name\":\"Aruba\",\"code\":\"AW\"},{\"name\":\"Australia\",\"code\":\"AU\"},{\"name\":\"Austria\",\"code\":\"AT\"},{\"name\":\"Azerbaijan\",\"code\":\"AZ\"},{\"name\":\"Bahamas\",\"code\":\"BS\"},{\"name\":\"Bahrain\",\"code\":\"BH\"},{\"name\":\"Bangladesh\",\"code\":\"BD\"},{\"name\":\"Barbados\",\"code\":\"BB\"},{\"name\":\"Belarus\",\"code\":\"BY\"},{\"name\":\"Belgium\",\"code\":\"BE\"},{\"name\":\"Belize\",\"code\":\"BZ\"},{\"name\":\"Benin\",\"code\":\"BJ\"},{\"name\":\"Bermuda\",\"code\":\"BM\"},{\"name\":\"Bhutan\",\"code\":\"BT\"},{\"name\":\"Bolivia\",\"code\":\"BO\"},{\"name\":\"Bosnia and Herzegovina\",\"code\":\"BA\"},{\"name\":\"Botswana\",\"code\":\"BW\"},{\"name\":\"Bouvet Island\",\"code\":\"BV\"},{\"name\":\"Brazil\",\"code\":\"BR\"},{\"name\":\"British Indian Ocean Territory\",\"code\":\"IO\"},{\"name\":\"Brunei Darussalam\",\"code\":\"BN\"},{\"name\":\"Bulgaria\",\"code\":\"BG\"},{\"name\":\"Burkina Faso\",\"code\":\"BF\"},{\"name\":\"Burundi\",\"code\":\"BI\"},{\"name\":\"Cambodia\",\"code\":\"KH\"},{\"name\":\"Cameroon\",\"code\":\"CM\"},{\"name\":\"Canada\",\"code\":\"CA\"},{\"name\":\"Cape Verde\",\"code\":\"CV\"},{\"name\":\"Cayman Islands\",\"code\":\"KY\"},{\"name\":\"Central African Republic\",\"code\":\"CF\"},{\"name\":\"Chad\",\"code\":\"TD\"},{\"name\":\"Chile\",\"code\":\"CL\"},{\"name\":\"China\",\"code\":\"CN\"},{\"name\":\"Christmas Island\",\"code\":\"CX\"},{\"name\":\"Cocos (Keeling) Islands\",\"code\":\"CC\"},{\"name\":\"Colombia\",\"code\":\"CO\"},{\"name\":\"Comoros\",\"code\":\"KM\"},{\"name\":\"Congo\",\"code\":\"CG\"},{\"name\":\"Congo, The Democratic Republic of the\",\"code\":\"CD\"},{\"name\":\"Cook Islands\",\"code\":\"CK\"},{\"name\":\"Costa Rica\",\"code\":\"CR\"},{\"name\":\"Cote D\\\"Ivoire\",\"code\":\"CI\"},{\"name\":\"Croatia\",\"code\":\"HR\"},{\"name\":\"Cuba\",\"code\":\"CU\"},{\"name\":\"Cyprus\",\"code\":\"CY\"},{\"name\":\"Czech Republic\",\"code\":\"CZ\"},{\"name\":\"Denmark\",\"code\":\"DK\"},{\"name\":\"Djibouti\",\"code\":\"DJ\"},{\"name\":\"Dominica\",\"code\":\"DM\"},{\"name\":\"Dominican Republic\",\"code\":\"DO\"},{\"name\":\"Ecuador\",\"code\":\"EC\"},{\"name\":\"Egypt\",\"code\":\"EG\"},{\"name\":\"El Salvador\",\"code\":\"SV\"},{\"name\":\"Equatorial Guinea\",\"code\":\"GQ\"},{\"name\":\"Eritrea\",\"code\":\"ER\"},{\"name\":\"Estonia\",\"code\":\"EE\"},{\"name\":\"Ethiopia\",\"code\":\"ET\"},{\"name\":\"Falkland Islands (Malvinas)\",\"code\":\"FK\"},{\"name\":\"Faroe Islands\",\"code\":\"FO\"},{\"name\":\"Fiji\",\"code\":\"FJ\"},{\"name\":\"Finland\",\"code\":\"FI\"},{\"name\":\"France\",\"code\":\"FR\"},{\"name\":\"French Guiana\",\"code\":\"GF\"},{\"name\":\"French Polynesia\",\"code\":\"PF\"},{\"name\":\"French Southern Territories\",\"code\":\"TF\"},{\"name\":\"Gabon\",\"code\":\"GA\"},{\"name\":\"Gambia\",\"code\":\"GM\"},{\"name\":\"Georgia\",\"code\":\"GE\"},{\"name\":\"Germany\",\"code\":\"DE\"},{\"name\":\"Ghana\",\"code\":\"GH\"},{\"name\":\"Gibraltar\",\"code\":\"GI\"},{\"name\":\"Greece\",\"code\":\"GR\"},{\"name\":\"Greenland\",\"code\":\"GL\"},{\"name\":\"Grenada\",\"code\":\"GD\"},{\"name\":\"Guadeloupe\",\"code\":\"GP\"},{\"name\":\"Guam\",\"code\":\"GU\"},{\"name\":\"Guatemala\",\"code\":\"GT\"},{\"name\":\"Guernsey\",\"code\":\"GG\"},{\"name\":\"Guinea\",\"code\":\"GN\"},{\"name\":\"Guinea-Bissau\",\"code\":\"GW\"},{\"name\":\"Guyana\",\"code\":\"GY\"},{\"name\":\"Haiti\",\"code\":\"HT\"},{\"name\":\"Heard Island and Mcdonald Islands\",\"code\":\"HM\"},{\"name\":\"Holy See (Vatican City State)\",\"code\":\"VA\"},{\"name\":\"Honduras\",\"code\":\"HN\"},{\"name\":\"Hong Kong\",\"code\":\"HK\"},{\"name\":\"Hungary\",\"code\":\"HU\"},{\"name\":\"Iceland\",\"code\":\"IS\"},{\"name\":\"India\",\"code\":\"IN\"},{\"name\":\"Indonesia\",\"code\":\"ID\"},{\"name\":\"Iran, Islamic Republic Of\",\"code\":\"IR\"},{\"name\":\"Iraq\",\"code\":\"IQ\"},{\"name\":\"Ireland\",\"code\":\"IE\"},{\"name\":\"Isle of Man\",\"code\":\"IM\"},{\"name\":\"Israel\",\"code\":\"IL\"},{\"name\":\"Italy\",\"code\":\"IT\"},{\"name\":\"Jamaica\",\"code\":\"JM\"},{\"name\":\"Japan\",\"code\":\"JP\"},{\"name\":\"Jersey\",\"code\":\"JE\"},{\"name\":\"Jordan\",\"code\":\"JO\"},{\"name\":\"Kazakhstan\",\"code\":\"KZ\"},{\"name\":\"Kenya\",\"code\":\"KE\"},{\"name\":\"Kiribati\",\"code\":\"KI\"},{\"name\":\"Korea, Democratic People\\\"S Republic of\",\"code\":\"KP\"},{\"name\":\"Korea, Republic of\",\"code\":\"KR\"},{\"name\":\"Kuwait\",\"code\":\"KW\"},{\"name\":\"Kyrgyzstan\",\"code\":\"KG\"},{\"name\":\"Lao People\\\"S Democratic Republic\",\"code\":\"LA\"},{\"name\":\"Latvia\",\"code\":\"LV\"},{\"name\":\"Lebanon\",\"code\":\"LB\"},{\"name\":\"Lesotho\",\"code\":\"LS\"},{\"name\":\"Liberia\",\"code\":\"LR\"},{\"name\":\"Libyan Arab Jamahiriya\",\"code\":\"LY\"},{\"name\":\"Liechtenstein\",\"code\":\"LI\"},{\"name\":\"Lithuania\",\"code\":\"LT\"},{\"name\":\"Luxembourg\",\"code\":\"LU\"},{\"name\":\"Macao\",\"code\":\"MO\"},{\"name\":\"Macedonia, The Former Yugoslav Republic of\",\"code\":\"MK\"},{\"name\":\"Madagascar\",\"code\":\"MG\"},{\"name\":\"Malawi\",\"code\":\"MW\"},{\"name\":\"Malaysia\",\"code\":\"MY\"},{\"name\":\"Maldives\",\"code\":\"MV\"},{\"name\":\"Mali\",\"code\":\"ML\"},{\"name\":\"Malta\",\"code\":\"MT\"},{\"name\":\"Marshall Islands\",\"code\":\"MH\"},{\"name\":\"Martinique\",\"code\":\"MQ\"},{\"name\":\"Mauritania\",\"code\":\"MR\"},{\"name\":\"Mauritius\",\"code\":\"MU\"},{\"name\":\"Mayotte\",\"code\":\"YT\"},{\"name\":\"Mexico\",\"code\":\"MX\"},{\"name\":\"Micronesia, Federated States of\",\"code\":\"FM\"},{\"name\":\"Moldova, Republic of\",\"code\":\"MD\"},{\"name\":\"Monaco\",\"code\":\"MC\"},{\"name\":\"Mongolia\",\"code\":\"MN\"},{\"name\":\"Montenegro\",\"code\":\"ME\"},{\"name\":\"Montserrat\",\"code\":\"MS\"},{\"name\":\"Morocco\",\"code\":\"MA\"},{\"name\":\"Mozambique\",\"code\":\"MZ\"},{\"name\":\"Myanmar\",\"code\":\"MM\"},{\"name\":\"Namibia\",\"code\":\"NA\"},{\"name\":\"Nauru\",\"code\":\"NR\"},{\"name\":\"Nepal\",\"code\":\"NP\"},{\"name\":\"Netherlands\",\"code\":\"NL\"},{\"name\":\"Netherlands Antilles\",\"code\":\"AN\"},{\"name\":\"New Caledonia\",\"code\":\"NC\"},{\"name\":\"New Zealand\",\"code\":\"NZ\"},{\"name\":\"Nicaragua\",\"code\":\"NI\"},{\"name\":\"Niger\",\"code\":\"NE\"},{\"name\":\"Nigeria\",\"code\":\"NG\"},{\"name\":\"Niue\",\"code\":\"NU\"},{\"name\":\"Norfolk Island\",\"code\":\"NF\"},{\"name\":\"Northern Mariana Islands\",\"code\":\"MP\"},{\"name\":\"Norway\",\"code\":\"NO\"},{\"name\":\"Oman\",\"code\":\"OM\"},{\"name\":\"Pakistan\",\"code\":\"PK\"},{\"name\":\"Palau\",\"code\":\"PW\"},{\"name\":\"Palestinian Territory, Occupied\",\"code\":\"PS\"},{\"name\":\"Panama\",\"code\":\"PA\"},{\"name\":\"Papua New Guinea\",\"code\":\"PG\"},{\"name\":\"Paraguay\",\"code\":\"PY\"},{\"name\":\"Peru\",\"code\":\"PE\"},{\"name\":\"Philippines\",\"code\":\"PH\"},{\"name\":\"Pitcairn\",\"code\":\"PN\"},{\"name\":\"Poland\",\"code\":\"PL\"},{\"name\":\"Portugal\",\"code\":\"PT\"},{\"name\":\"Puerto Rico\",\"code\":\"PR\"},{\"name\":\"Qatar\",\"code\":\"QA\"},{\"name\":\"Reunion\",\"code\":\"RE\"},{\"name\":\"Romania\",\"code\":\"RO\"},{\"name\":\"Russian Federation\",\"code\":\"RU\"},{\"name\":\"RWANDA\",\"code\":\"RW\"},{\"name\":\"Saint Helena\",\"code\":\"SH\"},{\"name\":\"Saint Kitts and Nevis\",\"code\":\"KN\"},{\"name\":\"Saint Lucia\",\"code\":\"LC\"},{\"name\":\"Saint Pierre and Miquelon\",\"code\":\"PM\"},{\"name\":\"Saint Vincent and the Grenadines\",\"code\":\"VC\"},{\"name\":\"Samoa\",\"code\":\"WS\"},{\"name\":\"San Marino\",\"code\":\"SM\"},{\"name\":\"Sao Tome and Principe\",\"code\":\"ST\"},{\"name\":\"Saudi Arabia\",\"code\":\"SA\"},{\"name\":\"Senegal\",\"code\":\"SN\"},{\"name\":\"Serbia\",\"code\":\"RS\"},{\"name\":\"Seychelles\",\"code\":\"SC\"},{\"name\":\"Sierra Leone\",\"code\":\"SL\"},{\"name\":\"Singapore\",\"code\":\"SG\"},{\"name\":\"Slovakia\",\"code\":\"SK\"},{\"name\":\"Slovenia\",\"code\":\"SI\"},{\"name\":\"Solomon Islands\",\"code\":\"SB\"},{\"name\":\"Somalia\",\"code\":\"SO\"},{\"name\":\"South Africa\",\"code\":\"ZA\"},{\"name\":\"South Georgia and the South Sandwich Islands\",\"code\":\"GS\"},{\"name\":\"Spain\",\"code\":\"ES\"},{\"name\":\"Sri Lanka\",\"code\":\"LK\"},{\"name\":\"Sudan\",\"code\":\"SD\"},{\"name\":\"Suriname\",\"code\":\"SR\"},{\"name\":\"Svalbard and Jan Mayen\",\"code\":\"SJ\"},{\"name\":\"Swaziland\",\"code\":\"SZ\"},{\"name\":\"Sweden\",\"code\":\"SE\"},{\"name\":\"Switzerland\",\"code\":\"CH\"},{\"name\":\"Syrian Arab Republic\",\"code\":\"SY\"},{\"name\":\"Taiwan, Province of China\",\"code\":\"TW\"},{\"name\":\"Tajikistan\",\"code\":\"TJ\"},{\"name\":\"Tanzania, United Republic of\",\"code\":\"TZ\"},{\"name\":\"Thailand\",\"code\":\"TH\"},{\"name\":\"Timor-Leste\",\"code\":\"TL\"},{\"name\":\"Togo\",\"code\":\"TG\"},{\"name\":\"Tokelau\",\"code\":\"TK\"},{\"name\":\"Tonga\",\"code\":\"TO\"},{\"name\":\"Trinidad and Tobago\",\"code\":\"TT\"},{\"name\":\"Tunisia\",\"code\":\"TN\"},{\"name\":\"Turkey\",\"code\":\"TR\"},{\"name\":\"Turkmenistan\",\"code\":\"TM\"},{\"name\":\"Turks and Caicos Islands\",\"code\":\"TC\"},{\"name\":\"Tuvalu\",\"code\":\"TV\"},{\"name\":\"Uganda\",\"code\":\"UG\"},{\"name\":\"Ukraine\",\"code\":\"UA\"},{\"name\":\"United Arab Emirates\",\"code\":\"AE\"},{\"name\":\"United Kingdom\",\"code\":\"GB\"},{\"name\":\"United States\",\"code\":\"US\"},{\"name\":\"United States Minor Outlying Islands\",\"code\":\"UM\"},{\"name\":\"Uruguay\",\"code\":\"UY\"},{\"name\":\"Uzbekistan\",\"code\":\"UZ\"},{\"name\":\"Vanuatu\",\"code\":\"VU\"},{\"name\":\"Venezuela\",\"code\":\"VE\"},{\"name\":\"Viet Nam\",\"code\":\"VN\"},{\"name\":\"Virgin Islands, British\",\"code\":\"VG\"},{\"name\":\"Virgin Islands, U.S.\",\"code\":\"VI\"},{\"name\":\"Wallis and Futuna\",\"code\":\"WF\"},{\"name\":\"Western Sahara\",\"code\":\"EH\"},{\"name\":\"Yemen\",\"code\":\"YE\"},{\"name\":\"Zambia\",\"code\":\"ZM\"},{\"name\":\"Zimbabwe\",\"code\":\"ZW\"}]", "COUNTRIES", null },
                    { 2, "[{\"UserId\":\"e992ded4-30ca-4cdd-9047-d7f0a5ab6378\",\"Count\":0,\"Name\":null}]", "V1_CONTRIBUTION_COUNT", null },
                    { 3, null, "OW_TILES", "7" },
                    { 4, null, "OW_CURRENT_EVENT", "default" },
                    { 5, "[{\"Name\":\"Ayutthaya\",\"Image\":\"EEA8BFCDB3B0890541E285A06B2576D1.jpg\"},{\"Name\":\"Black Forest\",\"Image\":\"9942992C23B965E66688836FFB7CDBA7.jpg\"},{\"Name\":\"Blizzard World\",\"Image\":\"03E2A086EB33ABDC923E74E7EB865B1E.jpg\"},{\"Name\":\"Busan\",\"Image\":\"149587D2F624F37CB82ABD80A4A4E41F.jpg\"},{\"Name\":\"Castillo\",\"Image\":\"B9FCA8911E8EF33090E62296FA3B2A53.jpg\"},{\"Name\":\"Château Guillard\",\"Image\":\"4AF3A88A9867C09CE419733CD57D61C4.jpg\"},{\"Name\":\"Dorado\",\"Image\":\"D3E82D82FAD1996F55594289A592A4E5.jpg\"},{\"Name\":\"Ecopoint: Antarctica\",\"Image\":\"96B6456DC240DE79F23479DA425C39BD.jpg\"},{\"Name\":\"Eichenwalde\",\"Image\":\"D9B1097537257D470425F224FB1E06ED.jpg\"},{\"Name\":\"Hanamura\",\"Image\":\"98D740E112498EE3ED4AFEA3D6E302D9.jpg\"},{\"Name\":\"Havana\",\"Image\":\"8819692009314E64E3EFB596442DBBB5.jpg\"},{\"Name\":\"Hollywood\",\"Image\":\"1AC441036F927B9815BA1137464EE064.jpg\"},{\"Name\":\"Horizon Lunar Colony\",\"Image\":\"40F075FBE8022F4F17B6987AC820F07D.jpg\"},{\"Name\":\"Ilios\",\"Image\":\"ECCF6E60594FBB46B0E3B05CBEAA108A.jpg\"},{\"Name\":\"Junkertown\",\"Image\":\"8DBF43786AF83F7A9F0D1202C4EE36F8.jpg\"},{\"Name\":\"Kanezaka\",\"Image\":\"87F12351D595167667B7C0E9ADEAD9C8.jpg\"},{\"Name\":\"King's Row\",\"Image\":\"0524E6C28D7CACED1F4B0237500ABE95.jpg\"},{\"Name\":\"Lijiang Tower\",\"Image\":\"CB2928846F35C2C6E94A10953C217354.jpg\"},{\"Name\":\"Necropolis\",\"Image\":\"9D58DF754FBB9C45417AD659DB8932EB.jpg\"},{\"Name\":\"Nepal\",\"Image\":\"7FEF6B003C726890EA5CA3708FE8FF56.jpg\"},{\"Name\":\"Numbani\",\"Image\":\"1E15A77E21723DCDEA8FA0836B19FC34.jpg\"},{\"Name\":\"Oasis\",\"Image\":\"0454236A66B299894FC9EE3F5E8FC6B6.jpg\"},{\"Name\":\"Paris\",\"Image\":\"E20D37A5D7FCC4C35BE6FC18A8E71BFA.jpg\"},{\"Name\":\"Petra\",\"Image\":\"A2289681DB3B897B364D0260F156C397.jpg\"},{\"Name\":\"Rialto\",\"Image\":\"0EB891CC6F9761E00EA26EB429897A8F.jpg\"},{\"Name\":\"Route 66\",\"Image\":\"AFC926AFD6ED6C27F102D1F4E46B94E6.jpg\"},{\"Name\":\"Temple of Anubis\",\"Image\":\"65348C0D09AE0A7F0D4C4E78C16193A1.jpg\"},{\"Name\":\"Volskaya Industries\",\"Image\":\"F7905086520CC03B66F532E333434C8C.jpg\"},{\"Name\":\"Watchpoint: Gibraltar\",\"Image\":\"21957EC2F7222326CB16EF5149818776.jpg\"}]", "OW_MAPS", null },
                    { 6, "[{\"Name\":\"Ana\",\"Image\":\"9ABA45A7F1999A9C5FC96EF2A45810FB.jpg\"},{\"Name\":\"Ashe\",\"Image\":\"42F4C2698438D8C3930E1673752C7BCE.jpg\"},{\"Name\":\"Baptiste\",\"Image\":\"04CE34A463C52D41C4D0C04C9AFD0ABE.jpg\"},{\"Name\":\"Bastion\",\"Image\":\"8811D3BA403C1C7FBE68ADE75125855C.jpg\"},{\"Name\":\"Brigitte\",\"Image\":\"74E6B44146AB714E66019C3609E67456.jpg\"},{\"Name\":\"D.Va\",\"Image\":\"6397BBC4231D2767170BDB0D78D4BBBA.jpg\"},{\"Name\":\"Doomfist\",\"Image\":\"47AABD3E1C5B2440DD55C74BBCFBBFD5.jpg\"},{\"Name\":\"Echo\",\"Image\":\"D31EF811301C6B4111380DDA959D6125.jpg\"},{\"Name\":\"Genji\",\"Image\":\"9080B834650206E96D8050B0228B4639.jpg\"},{\"Name\":\"Hanzo\",\"Image\":\"3682D76CEC61A12FFEA928CE0521E780.jpg\"},{\"Name\":\"Junkrat\",\"Image\":\"018A2AFA2B4F29BCB0E10AA479A8597A.jpg\"},{\"Name\":\"Lúcio\",\"Image\":\"B0463F69BB7FDE212EA46223BB7A9A39.jpg\"},{\"Name\":\"McCree\",\"Image\":\"36F1E3166916F01AE497A28B0C667301.jpg\"},{\"Name\":\"Mei\",\"Image\":\"F2FA6AF586895A4535F1C514C2AAC01B.jpg\"},{\"Name\":\"Mercy\",\"Image\":\"B91FAEDDD266FE5F6100C3C60CB5A22F.jpg\"},{\"Name\":\"Moira\",\"Image\":\"E5861973212BC2F7AC3F36B1EE5198E6.jpg\"},{\"Name\":\"Orisa\",\"Image\":\"618B699ACA80E3B2CE2BC147C4FF364F.jpg\"},{\"Name\":\"Pharah\",\"Image\":\"C1C29F54F3ABDFA711296B2E923F9FAD.jpg\"},{\"Name\":\"Reaper\",\"Image\":\"AC546F1FFF780B1B3D9B1E8E9BA9F6D9.jpg\"},{\"Name\":\"Reinhardt\",\"Image\":\"2D823E4915EA624B91984DCD8BF2BB4A.jpg\"},{\"Name\":\"Roadhog\",\"Image\":\"2729D3EF7FECCEEB3BC713A2A13BD087.jpg\"},{\"Name\":\"Sigma\",\"Image\":\"827FAFF1BDCFE446237C4D6289ABCE66.jpg\"},{\"Name\":\"Soldier: 76\",\"Image\":\"36B7FD0876D52BCF2BA5F8EA8DA30C5F.jpg\"},{\"Name\":\"Sombra\",\"Image\":\"75DF487B22B3318CA257E5E9D87624AB.jpg\"},{\"Name\":\"Symmetra\",\"Image\":\"F6603CCA1E3D4DAEA776314484ADFBF1.jpg\"},{\"Name\":\"Torbjörn\",\"Image\":\"E3D5B303923447E5D169A7B551CFBEA0.jpg\"},{\"Name\":\"Tracer\",\"Image\":\"FB4CF6037171E180A774F85E126F17FF.jpg\"},{\"Name\":\"Widowmaker\",\"Image\":\"67BCCB84B93D4F3A985117A70E531122.jpg\"},{\"Name\":\"Winston\",\"Image\":\"D64C37A2260DD6753EFB374C07A4463A.jpg\"},{\"Name\":\"Wrecking Ball\",\"Image\":\"622CE3F2B74D27432EBFD05668F96907.jpg\"},{\"Name\":\"Zarya\",\"Image\":\"FCD0CD07B55177EF32B88E97FDA1653C.jpg\"},{\"Name\":\"Zenyatta\",\"Image\":\"2E07634872CD6C8C5C4906095A8B979D.jpg\"}]", "OW_HEROES", null },
                    { 7, null, "OW2_TILES", "7" }
                });

            migrationBuilder.InsertData(
                table: "Contributors",
                columns: new[] { "Id", "Email", "Group", "Profile", "RegisteredAt", "Settings", "Username" },
                values: new object[] { new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"), "system@overwatcharcade.today", 0, null, new DateTime(2021, 7, 16, 20, 5, 35, 641, DateTimeKind.Utc).AddTicks(3610), null, "System" });

            migrationBuilder.InsertData(
                table: "Labels",
                columns: new[] { "Id", "Value" },
                values: new object[,]
                {
                    { 1, null },
                    { 2, "Daily" },
                    { 3, "Weekly" }
                });

            migrationBuilder.InsertData(
                table: "Dailies",
                columns: new[] { "Id", "ContributorId", "CreatedAt", "Game" },
                values: new object[] { 1, new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 });

            migrationBuilder.InsertData(
                table: "Dailies",
                columns: new[] { "Id", "ContributorId", "CreatedAt", "Game" },
                values: new object[] { 2, new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "TileMode",
                columns: new[] { "DailyId", "TileId", "ArcadeModeId", "LabelId" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 1, 2, 1, 2 },
                    { 1, 3, 1, 3 },
                    { 1, 4, 1, 1 },
                    { 1, 5, 1, 2 },
                    { 1, 6, 1, 3 },
                    { 1, 7, 1, 1 },
                    { 2, 1, 1, 1 },
                    { 2, 2, 1, 2 },
                    { 2, 3, 1, 3 },
                    { 2, 4, 1, 1 },
                    { 2, 5, 1, 2 },
                    { 2, 6, 1, 3 },
                    { 2, 7, 1, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Config_Key",
                table: "Config",
                column: "Key",
                unique: true,
                filter: "[Key] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dailies_ContributorId",
                table: "Dailies",
                column: "ContributorId");

            migrationBuilder.CreateIndex(
                name: "IX_TileMode_ArcadeModeId",
                table: "TileMode",
                column: "ArcadeModeId");

            migrationBuilder.CreateIndex(
                name: "IX_TileMode_LabelId",
                table: "TileMode",
                column: "LabelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "TileMode");

            migrationBuilder.DropTable(
                name: "Whitelist");

            migrationBuilder.DropTable(
                name: "ArcadeModes");

            migrationBuilder.DropTable(
                name: "Dailies");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "Contributors");
        }
    }
}
