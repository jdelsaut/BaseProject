namespace BoxApi.Persistance.Base
{
    public interface IDocumentCollectionContext<T>
    {
        string CollectionName { get; }
    }
}
