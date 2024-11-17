namespace JobPortal1.Models
    {
    public class PayFastSettings
        {
        public string MerchantId { get; set; }
        public string MerchantKey { get; set; }
        public string Passphrase { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string SandboxUrl { get; set; }
        }
    }
