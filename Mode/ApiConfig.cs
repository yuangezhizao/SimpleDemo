using ServiceStack.DataAnnotations;

namespace Mode
{
    public class ApiConfig
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }


    }
}
