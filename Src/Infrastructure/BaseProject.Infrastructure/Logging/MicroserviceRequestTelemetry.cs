namespace BaseProject.Infrastructure.Logging
{
    public class MicroserviceRequestTelemetry
    {
        public string Id { get; set; }
        public string RootId { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }

        public MicroserviceRequestTelemetry()
        {

        }
        public MicroserviceRequestTelemetry(string id, string name, string parentId, string rootId)
        {
            Id = id;
            Name = name;
            ParentId = parentId;
            RootId = rootId;
        }
    }
}
