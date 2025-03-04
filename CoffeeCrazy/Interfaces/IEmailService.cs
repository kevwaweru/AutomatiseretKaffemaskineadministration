﻿namespace CoffeeCrazy.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
        Task<bool> GenerateTokenAndSendResetEmail(string email);
    }
}
