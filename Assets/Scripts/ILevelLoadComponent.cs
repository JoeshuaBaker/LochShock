public interface ILevelLoadComponent 
{
    public string LoadLabel();
    public int LoadPriority();
    public void Load(World world);
}
