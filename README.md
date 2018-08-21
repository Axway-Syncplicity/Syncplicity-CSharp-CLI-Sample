# Syncplicity-CSharp-CLI-Sample

Shows examples of various API calls including the initial OAuth2 call.

## Description

This command-line sample app demonstrates various API calls including the initial OAuth2 authentication call.
This type of application would not support SSO-based authentication,
so would be the basis of an application typically used by administrator, not by a typical corporate user.

## System Requirements

* OS: Windows 7+
* .NET Framework: 4.5+
* Visual Studio: 2015+ (any edition)

## Usage

This sample application demonstrates usage of Syncplicity APIs. This is what you need to know or do before you begin to use Syncplicity APIs:

* Make sure you have an Enterprise Edition account you can use to login to the <https://developer.syncplicity.com>.
* First time login to Syncplicity:
  * You can log into Syncplicity Developer Portal using your Syncplicity login credentials.
    Only Syncplicity Enterprise Edition users are allowed to login to the Developer Portal.
    Based on the configuration done by your Syncplicity administrator,
    Syncplicity Developer Portal will present one of the following options for login:
    * Basic Authentication using Syncplicity username and password.
    * Enterprise Single Sign-on using the Web-SSO service used by your organization. We support ADFS, OneLogin, Ping and Okta.
* Once you have successfully logged in for the first time,
  the Syncplicity Developer Portal automatically creates an Enterprise Edition sandbox account to help you develop and test your application.
  Here is how it works:
  * The Syncplicity Developer Portal automatically creates your sandbox account
    by appending "-apidev" to the email address you used for logging into the Developer Portal.
    For e.g. if you logged into Syncplicity Developer Portal using user@domain.com as your email address,
    then your associated sandbox account email is user-apidev@domain.com.
  * The Developer Portal will prompt you to set your password for this sandbox account.
  * After you have successfully setup your password,
    you can use the sandbox email address and the newly configured password for logging into your sandbox account
    by visiting <https://my.syncplicity.com> and using "-apidev" email address.
    So, in the example above, you will have to use user-apidev@domain.com email address to log in to your sandbox account.
* Setup your developer sandbox account by configuring your password:
  * Login to your developer sandbox account by visiting <https://my.syncplicity.com> to make sure its correctly provisioned and that you can access it.
  * Through your user profile in the developer sandbox account,
    create an "Application Token" that you will need to authenticate yourself before making API calls.
    Learn more about this [here](https://syncplicity.zendesk.com/hc/en-us/articles/115002028926-Getting-Started-with-Syncplicity-APIs).
  * Review API documentation by visiting Docs page on the <https://developer.syncplicity.com>.
  * Register you app in the Developer Portal to obtain the "App Key" and "App Secret".
  
## Running

### Basic sample

1. Clone the sample project.
2. Use your favorite .NET IDE to open the `CSharpSampleApp.sln`.
3. Define new app on <https://developer.syncplicity.com>. The app key and app secret values are found in the application page.
  The Syncplicity admin token is found on the "My Account" page of the Syncplicity administration page.
  Use the "Application Token" field on that page to generate a token.
4. Update key values in `CSharpSampleApp/App.config`:
    * Update the the app key (`REPLACE_WITH_APP_KEY`).
    * Update the app secret (`REPLACE_WITH_APP_SECRET`).
    * Update the Syncplicity admin token (`REPLACE_WITH_ADMIN_TOKEN`).
    * Update the owner email, typically the sandbox owner email for development purposes (`REPLACE_OWNER_EMAIL`).
5. Build the solution.
6. Run the application.

### Storage Vault Authentication Sample

__Note:__ This is an advanced concept.
If your company does not use the SVA, you don't need to study it.
[Learn more about SVA.](https://syncplicity.zendesk.com/hc/en-us/articles/202659170-About-Syncplicity-StorageVaults-with-authentication-)

Working with Storage Vaults protected with SVA requires additional authentication procedures.
To run SVA sample:

1. Obtain **Storage Token**, **Machine Id** and **Machine Token**
    used to authenticate calls to Storage Vault.
    Follow the 'Setup Procedure' of the [Content Migration Guide](https://developer.syncplicity.com/content-migration-guide) to get those.
2. Configure the sample (`CSharpSampleApp\App.config`):
    1. Set Storage Token value (`REPLACE_WITH_STORAGE_TOKEN`)
    2. Set Machine Token value (`REPLACE_WITH_MACHINE_TOKEN`)
    3. Set Machine Id value (`REPLACE_WITH_MACHINE_ID`)
3. Build and run the sample application

### Debugging with Fiddler

To debug the sample app with Fiddler, one needs to make Fiddler SSL certificates trusted by the application.
The easiest way is by adding Fiddler's certificate to Trusted Root CA list.
To do this in Fiddler, go to Tools -> Options -> HTTPS -> Actions -> Trust Root Certificate.
Accept Fiddler warnings, reading carefully what they say.

## Team

![alt text][Axwaylogo] Axway Syncplicity Team

[Axwaylogo]: https://github.com/Axway-syncplicity/Assets/raw/master/AxwayLogoSmall.png  "Axway logo"

## License

Apache License 2.0
