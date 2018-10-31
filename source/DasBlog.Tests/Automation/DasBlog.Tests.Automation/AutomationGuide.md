#### Automation Guide

#### DasBlog Automation Components
We introduce a few concepts / class hierarchies to support automation.  These are inspired by the
automation framework in the Pluralsight course  [Automated Web Testing with Selenium and WebDriver Using Java by Bryan Hansen](https://app.pluralsight.com/library/courses/automated-web-testing-selenium-webdriver-java/table-of-contents)

* A **Browser** object abstracts the driver mechanisms and provides a simplified API to the other components.
* A set of **Page** objects encapsulate the properties and behaviour of the dasblog web pages.  These
components are unaware of the automation mechanism.
* Tests call into the pages to manipulate and retrieve information
* There is a set of **PageElement** objects which link the dasblog components to the Selenium web components.  It would
be possible to write these with or convert them to an approach independent of this specific automation mechanism
but this doesn't currently seem necessary.

#### Design Choices
* A key goal has been to enable contributors to create functional tests for the features they introduce
without the need for a deep understanding of Selenium.
* To that end the decision was made to identify all page elements by their id.  (Selenium has
myriad ways to identify elements which would entail a richer association with Selenium).
  This is slightly intrusive necessitating giving components
ids purely for testing purposes.

#### Development Workflow
1. The user will add a test to the existing functional or smoke tests.
2. __Either__ this will use existing functionality on the dasblog **Page** objects to carry out the test.
3. __Or__ new dasblog **PageElement**s will have to be added to the **Page** objects to reflect new elements added by the
feature under test
4. More infrequently, it may be necessary to add functionality to the **Browser** where new types of behaviour has been introduced
within the web app.
