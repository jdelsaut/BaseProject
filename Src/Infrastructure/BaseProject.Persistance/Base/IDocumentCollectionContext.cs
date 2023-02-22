namespace BaseProject.Persistance.Base
{
    public interface IDocumentCollectionContext<T>
    {
        string CollectionName { get; }
    }
}
