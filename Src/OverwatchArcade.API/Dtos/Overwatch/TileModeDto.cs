namespace OverwatchArcade.API.Dtos.Overwatch
{
    public class TileModeDto
    {
        public TileModeDto(string name, string players, string description, string image, string? label)
        {
            Name = name;
            Players = players;
            Description = description;
            Image = image;
            Label = label;
        }

        public string Name { get; set; }
        public string Players { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string? Label { get; set; }
    }
}
