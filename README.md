# SparkPost C# Webhooks Sample
---
## Prerequisites

Before using this example, you must have:

* A shiny new SparkPost Account, [sign up for a new account](https://app.sparkpost.com/) or [login to SparkPost](https://app.sparkpost.com/)

* .NET Core Runtime 2.0 installed [Download](https://dot.net/core)
## Installation

```
> cd SparkPostWebhooksSample
> dotnet restore
> dotnet ef database update
> dotnet run
```

## Testing The Webhook
* With fake data via [Postman](https://www.getpostman.com/postman)
* With real data via [ngrok](https://ngrok.com/) by creating a new [Webhook](https://app.sparkpost.com/account/webhooks) on your SparkPost Account that points to the ngrok tunnel address. (but first you need to run `ngrok http 5000` on your machine after running the project).

The sample runs on port 5000, so you need to make `POST` request to the webhook endpoint: [http://localhost:5000/api/webhook] with the expected payload.

Sample Payload:

* Webhook Ping:
```json 
[ { "msys": {} } ]
```
* Fake Data:
```json
[
        {
            "msys": {
                "message_event": {
                    "type": "bounce",
                    "bounce_class": "1",
                    "campaign_id": "Example Campaign Name",
                    "customer_id": "1",
                    "delv_method": "esmtp",
                    "device_token": "45c19189783f867973f6e6a5cca60061ffe4fa77c547150563a1192fa9847f8a",
                    "error_code": "554",
                    "event_id": "92356927693813856",
                    "friendly_from": "sender@example.com",
                    "ip_address": "127.0.0.1",
                    "ip_pool": "Example-Ip-Pool",
                    "message_id": "000443ee14578172be22",
                    "msg_from": "sender@example.com",
                    "msg_size": "1337",
                    "num_retries": "2",
                    "rcpt_meta": {
                        "customKey": "customValue"
                    },
                    "rcpt_tags": [
                        "male",
                        "US"
                    ],
                    "rcpt_to": "recipient@example.com",
                    "raw_rcpt_to": "recipient@example.com",
                    "rcpt_type": "cc",
                    "raw_reason": "MAIL REFUSED - IP (17.99.99.99) is in black list",
                    "reason": "MAIL REFUSED - IP (a.b.c.d) is in black list",
                    "routing_domain": "example.com",
                    "sending_ip": "127.0.0.1",
                    "sms_coding": "ASCII",
                    "sms_dst": "7876712656",
                    "sms_dst_npi": "E164",
                    "sms_dst_ton": "International",
                    "sms_src": "1234",
                    "sms_src_npi": "E164",
                    "sms_src_ton": "Unknown",
                    "subaccount_id": "101",
                    "subject": "Summer deals are here!",
                    "template_id": "templ-1234",
                    "template_version": "1",
                    "timestamp": "1454442600",
                    "transmission_id": "65832150921904138"
                }
            }
        }
]
```