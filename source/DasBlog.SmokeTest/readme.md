#### Smoke Test

##### Usage
cd to <project>/source/DasBlog.SmokeTest and do `dotnet run`.  The dasblog-core is kicked off (on a port of 5000)
and something like the following will show in the console or debug window:



    info: DasBlog.SmokeTest.App
    Current directory at start up is C:\projects\dasblog-core\source\DasBlog.SmokeTest
    info: DasBlog.SmokeTest.App[0]
      Started App
    1532783198757   geckodriver     INFO    geckodriver 0.21.0
    1532783198766   geckodriver     INFO    Listening on 127.0.0.1:65346
    DasBlog.Web says: Hosting environment: Development
    DasBlog.Web says: Content root path: c:\projects\dasblog-core\source\DasBlog.Web.UI
    DasBlog.Web says: Now listening on: http://localhost:5000
    DasBlog.Web says: Application started. Press Ctrl+C to shut down.
    1532783203310   mozrunner::runner       INFO    Running command: "C:\\Program Files\\Mozilla Firefox\\firefox.exe" "-marionette" "-foreground" "-no-remote" "-profile" "C:\\Users\\MIKEDA~1\\AppData\\Local\\Temp\\rust_mozprofile.
    zFA4OckiEGBj"`

The lines staring 'Info' originate rom DasBlog.SmokeTest.  The lines starting with a number
come from the in-process (?) Firefox driver and those beginning 
"DasBlog.Web says:" come from the the web app that we know and love.
