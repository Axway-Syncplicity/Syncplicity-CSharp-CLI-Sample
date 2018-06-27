# Syncplicity-CSharp-SLI-Sample

## Description
This sample app is similar to the Java command-line sample app but has been extended to show how a developer might invoke various APIs against Active Directory (AD) to provision users and assign those users to various user groups based on the metadata that is configured inside AD.


## Usage
This sample application demonstrates usage of Syncplicity APIs. This is what you need to know or do before you begin to use Syncoplicity APIs:
* Make sure you have an Enterprise Edition account you can use to login to the https://developer.syncplicity.com.
 * First time login to Syncplicity:
   * You can log into Syncplicity Developer Portal using your Syncplicity login credentials. Only Syncplicity Enterprise Edition users are allowed to login to the Developer Portal. Based on the configuration done by your Syncplicity administrator, Syncplicity Developer Portal will present one of the following options for login:
    * Basic Authentication using Syncplicity username and password.
    * Enterprise Single Sign-on using the Web-SSO service used by your organization. We support ADFS, OneLogin, Ping and Okta.
   * Once you have successfully logged in for the first time, the Syncplicity Developer Portal automatically creates an Enterprise Edition sandbox account to help you develop and test your application. Here is how it works:
    * The Syncplicity Developer Portal automatically creates your sandbox account by appending "-apidev" to the email address you used for logging into the Developer Portal. For e.g. if you logged into Syncplicity Developer Portal using user@domain.com as your email address, then your associated sandbox account email is user-apidev@domain.com.
The Developer Portal will prompt you to set your password for this sandbox account. See screenshot below for reference.
    * The Developer Portal will prompt you to set your password for this sandbox account. See screenshot below for reference.
    * After you have successfully setup your password, you can use the sandbox email address and the newly configured password for logging into your sandbox account by visiting https://my.eu.syncplicity.com and using "-apidev" email address. So, in the example above, you will have to use "user-apidev@domain.com" email address to log in to your sandbox account
* Setup your developer sandbox account by configuring your password:
  * Login to your developer sandbox account by visiting https://my.eu.syncplicity.com to make sure its correctly provisioned and that you can access it.
  * Login to your developer sandbox account by visiting https://my.eu.syncplicity.com to make sure its correctly provisioned and that you can access it.
  * Through your user profile in the developer sandbox account, create an "Application Token" that you will need to authenticate yourself before making API calls. Learn more about this here.
  * Review API documentation by visiting Docs page on the https://developer.syncplicity.com.
  * Register you app in the Developer Portal to obtain the "App Key" and "App Secret". 
  
### Deploy
Clone the sample project.
Use your favorite Java IDE to open the .project file provided in the sub-directories
Define new app on https://developer.syncplicity.com. The app key and app secret values are found in the application page. The Syncplicity admin token is found on the "My Account" page of the Syncplicity administration page. Use the "Application Token" field on that page to generate a token.
Update key values in java\com\syncplicity\api\samples\commonesources\config.properties:
* Update the the consumer key value (REPLACE_WITH_APP_KEY)
* Update the consumer secret (REPLACE_WITH_APP_SECRET)
* Update the Syncplicity admin token (REPLACE_WITH_ADMIN_TOKEN)
* Update the EE account owner email, typically the sandbox owner email for development purposes (REPLACE_OWNER_EMAIL_OF_SANDBOX_EE_ACCOUNT)

Run the application.

## Contributing
See contribute.md file in the root directory if you want to submit an issue or add new sample use case etc.

## Team

![alt text][Axwaylogo] Axway Syncplicity Team

[Axwaylogo]: https://github.com/Axway-API-Management/Common/blob/master/img/AxwayLogoSmall.png  "Axway logo"


## License
Apache License 2.0
