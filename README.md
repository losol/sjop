


```
dotnet user-secrets set "StripeSettings:SecretKey" "sk_test_asdfasdf"
dotnet user-secrets set "StripeSettings:PublishableKey" "pk_test_asdfasdf"
dotnet user-secrets set "StripeSettings:WebhookSecret" "whsec_asdf"
```


```
stripe listen --forward-to localhost:5000/webhooks/stripe
```