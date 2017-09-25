﻿using System;
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

            var extracted_emails = payload.OfType<JObject>()
                                          .Select(obj => obj["msys"]).Cast<JObject>()
                                          .Where(msys => msys.Properties().FirstOrDefault() != null)
                                          .Select(msys => msys.Properties().FirstOrDefault().Value).Cast<JObject>()
                                          .Where(evt => unsubscribe_events.Contains(evt["type"].ToString()))
                                          .Select(msg => msg["rcpt_to"].ToString())
                                          .Distinct()
                                          .ToList();

            foreach (var email in extracted_emails)
            {
                var subscriber = _db.Subscribers.FirstOrDefault(s => s.Email == email);
                if (subscriber != null)
                {
                    subscriber.Subscribed = false;
                }
            }

            await _db.SaveChangesAsync();

            return Ok(extracted_emails);
        }
    }
}