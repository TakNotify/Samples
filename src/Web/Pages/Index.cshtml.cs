using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TakNotify;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly INotification _notification;

        public IndexModel(INotification notification, ILogger<IndexModel> logger)
        {
            _notification = notification;
            _logger = logger;
        }

        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public IEnumerable<WeatherForecast> WeatherData { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Your Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public void OnGet()
        {
            if (TempData["Email"] != null)
            {
                Email = TempData["Email"] as string;
            }

            if (TempData["SuccessMessage"] != null)
            {
                Message = TempData["SuccessMessage"] as string;
                IsSuccess = true;
            }

            if (TempData["ErrorMessage"] != null)
            {
                Message = TempData["ErrorMessage"] as string;
                IsSuccess = false;
            }

            if (TempData["WeatherData"] != null)
            {
                WeatherData = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(TempData["WeatherData"] as string);
            }
            else
            {
                WeatherData = WeatherForecast.GetData();
            }
        }

        public async Task<IActionResult> OnPostSmtp(string data)
        {
            var message = new EmailMessage()
            {
                ToAddresses = new List<string> { Email },
                Subject = "[TakNotify - SMTP] Weather Forecast",
                Body = $"Forecast: {data}"
            };

            var result = await _notification.SendEmailWithSmtp(message);

            TempData["Email"] = Email;
            TempData["WeatherData"] = data;

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Email notification was sent to {string.Join(", ", message.ToAddresses)} via SMTP";
                _logger.LogDebug("Email notification was sent to {toAddresses} via SMTP", message.ToAddresses);
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to send notification to {string.Join(", ", message.ToAddresses)} via SMTP. Please see the log.";
                _logger.LogWarning("Failed to send notification to {toAddresses} via SMTP. Error: {error}", message.ToAddresses, result.Errors);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendGrid(string data)
        {
            var message = new SendGridMessage()
            {
                ToAddresses = new List<string> { Email },
                Subject = "[TakNotify - SendGrid] Weather Forecast",
                PlainContent = $"Forecast: {data}"
            };

            var result = await _notification.SendEmailWithSendGrid(message);

            TempData["Email"] = Email;
            TempData["WeatherData"] = data;

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Email notification was sent to {string.Join(", ", message.ToAddresses)} via SendGrid";
                _logger.LogDebug("Email notification was sent to {toAddresses} via SendGrid", message.ToAddresses);
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to send notification to {string.Join(", ", message.ToAddresses)} via SendGrid. Please see the log.";
                _logger.LogWarning("Failed to send notification to {toAddresses} via SendGrid. Error: {error}", message.ToAddresses, result.Errors);
            }

            return RedirectToPage();
        }
    }
}
