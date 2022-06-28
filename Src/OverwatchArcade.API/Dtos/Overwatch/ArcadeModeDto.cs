namespace OverwatchArcade.API.Dtos.Overwatch
{
    public class ArcadeModeDto
    {
        public ArcadeModeDto(int id, string name, string players, string description, string image)
        {
            Id = id;
            Name = name;
            Players = players;
            Description = description;
            Image = image;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Players { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
