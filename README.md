[![Build status](https://dev.azure.com/losolio/LosvikKommune/_apis/build/status/LosvikKommune%20-%20Azure%20-%20CI)](https://dev.azure.com/losolio/LosvikKommune/_build/latest?definitionId=15)

# Sjop

```
dotnet user-secrets set "StripeSettings:SecretKey" "sk_test_asdfasdf"
dotnet user-secrets set "StripeSettings:PublishableKey" "pk_test_asdfasdf"
dotnet user-secrets set "StripeSettings:WebhookSecret" "whsec_asdf"
```

```
stripe listen --forward-to localhost:5000/webhooks/stripe
```


Set up ngrok to test webhooks. 
``` 
Win: ..\ngrok http 5000
```