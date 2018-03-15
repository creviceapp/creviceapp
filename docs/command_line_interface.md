
# Comamand line interface

```
Usage:
  crevice4.exe [--nogui] [--script path] [--help]

  -g, --nogui       (Default: False) Disable GUI features. Set to true if you 
                    use Crevice as a CUI application.

  -n, --nocache     (Default: False) Disable user script assembly caching. 
                    Strongly recommend this value to false because compiling 
                    task consumes CPU resources every startup of application if
                    true.

  -s, --script      (Default: default.csx) Path to user script file. Use this 
                    option if you need to change the default location of user 
                    script. If given value is relative path, Crevice will 
                    resolve it to absolute path based on the default directory 
                    (%USERPROFILE%\AppData\Roaming\Crevice4).

  -p, --priority    (Default: High) Process priority. Acceptable values are the
                    following: AboveNormal, BelowNormal, High, Idle, Normal, 
                    RealTime.

  -V, --verbose     (Default: False) Show details about running application.

  -v, --version     (Default: False) Display product version.

  --help            Display this help screen.
```
