namespace JwtTokenTask.Helper
{
    public class JwtClass
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public Double DurationInDays { get; set; }
    }
}
