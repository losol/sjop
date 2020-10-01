[![Build status](https://dev.azure.com/losolio/LosvikKommune/_apis/build/status/LosvikKommune%20-%20Azure%20-%20CI)](https://dev.azure.com/losolio/LosvikKommune/_build/latest?definitionId=15)

# Sjop

Sjop is an ecommerce solution written in ASP.NET Core 3.1. 


## Setting up Payment providers. 


### Stripe
After [getting an Stripe account](https://dashboard.stripe.com/register) you should save your API keys so that the app could reach it. The best way is by using `dotnet user-secrets`, which will override the settings in `appsettings.json`. 

```
dotnet user-secrets set "Stripe:SecretKey" "sk_test_asdfasdf"
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_asdfasdf"
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_asdf"
```

### Vipps
For norwegian webshops Vipps payments is supported. 

```
dotnet user-secrets set "Vipps:MerchantSerialNumber" "YOUR_INFO"
dotnet user-secrets set "Vipps:ClientId" "YOUR_INFO"
dotnet user-secrets set "Vipps:ClientSecret" "YOUR_INFO"
dotnet user-secrets set "Vipps:SubscriptionKey" "YOUR_INFO"

```

## Test webhooks

After Stripe has received payment, it sends an webhook to the application which triggers a status change on the order to `paid`. Two possible solutions for letting Stripe reach localhost. 

### Using Stripe CLI

After installing Stripe CLI. Use port 5000 to avoid SSL certificate issues. 

```
stripe listen --forward-to localhost:5000/webhooks/stripe
```

### Using ngrok

Set up ngrok to test webhooks. Use port 5000, as SSL forwarding is a premium feature. 

```
Win: ..\ngrok http 5000
```
