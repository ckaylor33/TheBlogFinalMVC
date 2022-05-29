using System;

namespace TheBlogFinalMVC.ViewModels
{
    public class MailSettings
    {
        //So we can use and configure an smtp server
        //from google for example
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password  { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
