### Contributor FAQ
Created August 2018

#### How is dasBlog typically deployed? Where?
By individuals... ## ***** ??

#### Is dasBlog designed to run in a multi-tenanted environment? 
No

#### How are categories stored? 
As attributes of blog post entries.  There is no separate data store for categories

#### What are Themes? 
They are skins which decorate the main blog post view pages.  See Configuration->Default Theme
in the legacy dasBlog application

#### What is the ambition/goal/roadmap - reproduction of DasBlog?, mutl-ilanguage?, blogRoll? MVP?
If we're doing multi-language then that may need to be plumbed in sooner rather than later.

#### What is the role of macros in the legacy DasBlog application?
## ***** ??


#### Is dasBlog (original) web forms or MVC based?
Web forms

#### Relationship between DasBlog.Core.Configurations.ISiteConfig and newtelligence.DasBlog.Web.SiteConfig?
They refer to the same set of data but are otherwise unconnected

#### How does site.config get populated?
ASP.NET core magic.  An implementation of IConfigurationBuilder reads in all the config files (which
rely on key-value pairs) and builds a great big dictionary.  Then
 multiple calls to IServiceCollection.Configure dole out the settings to various 
 objects including SiteConfig reflecting on matching names.  As far as I'm concerned it's a gateway drug

#### How does local login work?
Microsofoft.AspNetCore.Identity.SignInManager.PasswordSignInAsync relies on components of the identity
framework to route login requests to DasBlog.Web.Identity.DasBlogUserStore.  The identity
framework identifies the datastore through some marker interfaces it esposes such as
IUserPasswordStore

By convention the password for dasblog-core is "admin".  Look at DasBlog.Web.UI/config/siteSecurity.config
for available users.

The username/password for legacy DasBlog is "admin/admin"

#### What are the respective roles of DasBlogUser and Security.User
Security.User is the cental object in user management.  DasBlogUser is part of the identity framework.
We have not fully brought the two classes together.


#### Is there any preference between razor pages and javascript and tag helpers?
Tag helpers are used fairly extensively and are used to wrap the javascript

#### Is compatibility with the sites of existing dasBlog users a requirement?

## **** ????

#### What are the naming and formatting standards for the code?
We favour Microsoft C# coding standards.  See TagHelperTest for the test naming standard

#### Github workflow
Pull request workflow:https://gist.github.com/Chaser324/ce0505fbed06b947d962

#### Resources
Legacy DasBlog

https://github.com/poppastring/dasblog-core
