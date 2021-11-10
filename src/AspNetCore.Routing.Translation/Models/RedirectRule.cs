namespace AspNetCore.Routing.Translation.Models
{
    public class RedirectRule
    {
        public string Regex { get; set; }
        
        public string Replacement { get; set; }

        public int? StatusCode { get; set; }
    }
}