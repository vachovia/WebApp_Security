﻿namespace WebApp.Settings
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string From { get; set; } = string.Empty;
        public string User {  get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
