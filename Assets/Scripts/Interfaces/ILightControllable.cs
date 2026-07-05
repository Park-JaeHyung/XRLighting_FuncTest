public interface ILightControllable
{
    int Id { get; }
    LightParameters GetParameters();
    void ApplyParameters(LightParameters parameters);
}
