namespace JwtTokenTask.ViewModel
{
    public class AuthModel
    {
        public string Massage { get; set; }
        public bool IsAuthontocated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
    }
}
