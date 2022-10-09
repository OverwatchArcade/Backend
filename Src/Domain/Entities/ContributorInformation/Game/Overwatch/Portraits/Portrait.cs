namespace OverwatchArcade.Domain.Entities.ContributorInformation.Game.Overwatch.Portraits;

public abstract class Portrait
{
    protected Portrait(string name, string image)
    {
        Name = name;
        Image = image;
    }

    public string Name { get; set; }
    public string Image { get; set; }
}