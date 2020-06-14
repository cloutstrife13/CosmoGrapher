using CosmoGrapher.Classes;

namespace CosmoGrapher
{
    public class DbContextGremlin
    {
        public Graph DefaultModel = new Graph(
            "AzureDbName",
            "AzureDbGroupName",
            "AzureDbGroupMemberName",
            "AzurePrimaryKey",
            "AzurePartitionKey"
        );

        //public static IEdmModel GetEdmModel()
        //{
        //    var builder = new ODataConventionModelBuilder();

        //    return builder.GetEdmModel();
        //}
    }
}