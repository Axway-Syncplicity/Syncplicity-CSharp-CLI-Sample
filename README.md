# Syncplicity-CSharp-CLI-Sample

Shows examples of various API calls including the initial OAuth2 call.

## Description

This sample application demonstrates various API calls including the initial OAuth2 authentication call.
This is a CLI application that does not support SSO-based authentication,
so would be the basis of an application typically used by administrator, not by a regular Syncplicity user.

## System Requirements

* OS: Windows 7+
* .NET Framework: 4.5+
* Visual Studio: 2015+ (any edition)

## Usage

This sample application demonstrates usage of Syncplicity APIs. This is what you need to know or do before you begin to use Syncplicity APIs:

* Make sure you have an Enterprise Edition account you can use to login to the Developer Portal at <https://developer.syncplicity.com>.
* Log into Syncplicity Developer Portal using your Syncplicity login credentials.
  Only Syncplicity Enterprise Edition users are allowed to login to the Developer Portal.
  Based on the configuration done by your Syncplicity administrator,
  Syncplicity Developer Portal will present one of the following options for login:
  * Basic Authentication using Syncplicity username and password.
  * Enterprise Single Sign-on using the web SSO service used by your organization. We support ADFS, OneLogin, Ping and Okta.
* Once you have successfully logged in for the first time,
  you must create an Enterprise Edition sandbox account in the Developer Portal.
  This account can be used to safely test your application using all Syncplicity features
  without affecting your company production data.
  * Log into Syncplicity Developer Portal. Click 'My Profile' and then 'Create sandbox'.
    Refer to the documentation for guidance: <https://developer.syncplicity.com/documentation/overview>.
  * You can log into <https://my.syncplicity.com> using the sandbox account.
    Note that the sandbox account email has "-apidev" suffix.
    So, assuming you regular account email is user@domain.com,
    use user-apidev@domain.com email address to log in to your sandbox account.
* Setup your developer sandbox account:
  * Log into the sandbox account at <https://my.syncplicity.com> to make sure its correctly provisioned and that you can access it.
  * Go to the 'Account' menu.
  * Click "Create" under "Application Token" section.
    The token is used to authenticate an application before making API calls.
    Learn more [here](https://syncplicity.zendesk.com/hc/en-us/articles/115002028926-Getting-Started-with-Syncplicity-APIs).
* Review API documentation by visiting documentation section on the <https://developer.syncplicity.com>.
* Register you application in the Developer Portal to obtain the "App Key" and "App Secret".

## Running

### Basic sample

1. Clone the sample project.
2. Use your favorite .NET IDE to open the `CSharpSampleApp.sln`.
3. Define new application on <https://developer.syncplicity.com>. The app key and app secret values are found in the application page.
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

### Running On-Behalf-Of sample (As User)

The On Behalf Of sample demonstrates how an administrator can execute actions on behalf of other users (impersonating other users).
Running the On Behalf Of sample requires additional configuration.

You need to specify the email of the impersonated user in the `asUserEmail` parameter in `App.config`.

Besides, the owner of the Application Token must have permissions to execute code on behalf of other users.
By default, Global Administrators do not have this permission. To grant this permission:

1. There must be at least two Global Administrator users in the company.
2. One administrator must sign into Syncplicity (<https://my.syncplicity.com>)
3. Go to the Admin area, User Accounts
4. Find the other administrator account
5. Under "Privileges", click "Modify", select "Access content on behalf of managed users through API" and click "Save"
6. Confirm notification of all administrators about the action

Once this is done, the second administrator account can use the `As User` parameter.

### Debugging with Fiddler

To debug the sample application with Fiddler, one needs to make Fiddler SSL certificates trusted by the application.
The easiest way is by adding Fiddler's certificate to Trusted Root CA list.
To do this in Fiddler, go to Tools -> Options -> HTTPS -> Actions -> Trust Root Certificate.
Accept Fiddler warnings, reading carefully what they say.

## Team

![alt text][Axwaylogo] Axway Syncplicity Team

[Axwaylogo]: https://github.com/Axway-syncplicity/Assets/raw/master/AxwayLogoSmall.png  "Axway logo"

## License

Apache License 2.0
