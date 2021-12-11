namespace OWArcadeBackend.Dtos.Overwatch
{
    public class TileModeDto
    {
        public string Name { get; set; }
        public string Players { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        #nullable enable
        public string? Label { get; set; }
    }
}
