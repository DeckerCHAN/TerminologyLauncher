namespace AppoverHelper.Structure
{
    public class NuGetFeed
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public int ProjectId { get; set; }
        public bool PublishingEnabled { get; set; }
        public string Created { get; set; }
    }
}