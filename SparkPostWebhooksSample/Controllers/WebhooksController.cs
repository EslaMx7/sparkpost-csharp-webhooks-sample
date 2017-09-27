using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SparkPostWebhooksSample.Models;

namespace SparkPostWebhooksSample.Controllers
{
    [Route("api/webhook")]
    public class WebhooksController : Controller
    {
        private readonly SubscribersDbContext _db;

        public WebhooksController(SubscribersDbContext subscribersDb)
        {
            _db = subscribersDb;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveEventsAsync([FromBody] JArray payload)
        {
            var unsubscribe_events = new string[] { "bounce", "list_unsubscribe", "spam_complaint", "out_of_band", "link_unsubscribe" };

            if (!ModelState.IsValid)
                return BadRequest();

            var msys_object = payload.OfType<JObject>()
                                          .Select(obj => obj["msys"]).Cast<JObject>(); // We are stripping msys object

            var event_object = msys_object.Where(msys => msys.Properties().FirstOrDefault() != null) // Filtering out pings
                                          .Select(msys => msys.Properties().FirstOrDefault().Value).Cast<JObject>(); // Stripping the first *_event structure

            var subscribers_emails = event_object.Where(evt => unsubscribe_events.Contains(evt["type"].ToString())) // Filter on event type
                                                 .Select(msg => msg["rcpt_to"].ToString()); // Extract subscriber email

            var extracted_emails = subscribers_emails.Distinct() // Remove emails duplication
                                                     .ToList();


            foreach (var email in extracted_emails)
            {
                var subscriber = _db.Subscribers.FirstOrDefault(s => s.Email == email); // Check if the email already exist
                if (subscriber != null)
                {
                    subscriber.Subscribed = false; // Unsubscribe
                }
            }

            await _db.SaveChangesAsync();

            return Ok(extracted_emails);
        }

        [Route("/")]
        [HttpGet]
        public IActionResult Home()
        {
            return Json(new { message = "Your Webhook Is Up And Running. You Can Try It By Sending [POST] Requests To: '/api/webhook'." });
        }
    }
}