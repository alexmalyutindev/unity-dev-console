DevConsole for Unity
==========

<p align="center">
<img width="600" alt="demo" src="https://github.com/user-attachments/assets/c6cc23d2-c46f-4d5b-ac8a-d52ffb18c252">
</p>

DevConsole is an easy-to-use tool for making you custom commands to run them in your game for debugging purpose.

Example
-------
```cs
// Initialization
var devConsole = new DevConsole();
var view = new DevConsoleView(_devConsole);

// Command declaration
[DevCmd(name: "Echo", help: "write argument to the debug console")]
private static void Echo(string message) => Debug.Log(message);

// Command registration
devConsole.Register<string>(Echo);

// Console rendering (see: DevConsoleRenderer.cs)
private void OnGUI() => view.OnGUI();

```

Installation
------------
Find the manifest.json file in the Packages folder of your project and add a line to `dependencies` field:

* ```"com.alexmalyutindev.urp-ssr": "https://github.com/alexmalyutindev/unity-dev-console.git"```

Or, you can add this package using PackageManager `Add package from git URL` option:


* ```https://github.com/alexmalyutindev/unity-dev-console.git```


License
-------
This project is MIT License - see the [LICENSE](LICENSE) file for details
