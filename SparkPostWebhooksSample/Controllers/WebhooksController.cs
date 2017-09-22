using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace SparkPostWebhooksSample.Controllers
{
    [Route("api/webhook")]
    public class WebhooksController : Controller
    {
        [HttpPost]
        public IActionResult ReceiveEvents([FromBody] JArray payload)
        {

            var unsubscribe_events = new string[] {  "bounce", "list_unsubscribe", "spam_complaint", "out_of_band", "link_unsubscribe" };

            var extracted_emails = new HashSet<string>();

            foreach (var obj in payload)
            {
                var msys = obj["msys"] as JObject;

                var evt_prop = msys.Properties().FirstOrDefault();

                var message_paylod = msys[evt_prop?.Name] as JObject;

                var message_type = message_paylod.Property("type")?.Value.ToString();

                if (unsubscribe_events.Contains(message_type))
                {
                    var recipient_address = message_paylod.Property("rcpt_to")?.Value.ToString();

                    extracted_emails.Add(recipient_address);
                }
            }

            // TODO: unsubscribe

            return Ok(extracted_emails);
        }
    }
}