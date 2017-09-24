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
            var extracted_emails = new HashSet<string>();

            if (!ModelState.IsValid)
                return BadRequest();

            foreach (var obj in payload)
            {
                var msys = obj["msys"] as JObject;
                var event_type = msys?.Properties().FirstOrDefault();
                if (event_type != null)
                {
                    var message_paylod = msys[event_type.Name] as JObject;
                    var message_type = message_paylod.Property("type")?.Value.ToString();
                    if (unsubscribe_events.Contains(message_type))
                    {
                        var recipient_address = message_paylod.Property("rcpt_to")?.Value.ToString();
                        extracted_emails.Add(recipient_address);
                    }
                }
            }

            foreach (var email in extracted_emails)
            {
                var subscriber = _db.Subscribers.FirstOrDefault(s=>s.Email == email);
                if(subscriber != null)
                {
                    subscriber.Subscribed = false;
                }
            }

            await _db.SaveChangesAsync();

            return Ok(extracted_emails);
        }
    }
}