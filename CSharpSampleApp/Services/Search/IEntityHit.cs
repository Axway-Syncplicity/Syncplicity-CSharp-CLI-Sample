namespace CSharpSampleApp.Services.Search
{
    public interface IEntityHit
    {
        double Rank { get; }

        string Name { get; }
    }
}
