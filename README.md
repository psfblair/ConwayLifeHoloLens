# Conway's Game of Life for HoloLens

## An attempt...

This is a Unity3D project, with code written in F#, for 
Conway's Game of Life on the Microsoft HoloLens. Unfortunately,
because of [this bug](https://fogbugz.unity3d.com/default.asp?894364_ejukbrck4aott5h9),
the code won't run on HoloLens, and Unity has no plans to fix the bug.
You may want to upvote F# support for Unity [here](https://feedback.unity3d.com/suggestions/f-support).

An introduction to this project is contained in the slides at the root of this
project (AlternativeToolchains.pdf). More background is available in the following 
blog posts:

* ["Yes, Virginia, You Can Write HoloLens Apps in F#"](http://seriouscodeblog.blogspot.ca/2017/03/yes-virginia-you-can-write-hololens.html)
* ["Some Gotchas Writing Unity Apps in F#"](http://seriouscodeblog.blogspot.ca/2017/03/some-gotchas-writing-unity-apps-in-f.html)
* ["No, Virginia, You Can't Actually Write Unity Apps for HoloLens in F#"](http://seriouscodeblog.blogspot.ca/2017/04/no-virginia-you-cant-actually-write.html)
* ["...and You Can Write HoloLens Apps in macOS / Linux Too (Part 1: The Cloud Server)"](http://seriouscodeblog.blogspot.ca/2017/03/write-hololens-apps-on-macos-linux-1.html)
* ["...and You Can Write HoloLens Apps in macOS / Linux Too (Part 2: Project Setup)"](http://seriouscodeblog.blogspot.ca/2017/03/write-hololens-apps-on-macos-linux-2.html)
* ["...and You Can Write HoloLens Apps in macOS / Linux Too (Part 3: Deploy to HoloLens)"](http://seriouscodeblog.blogspot.ca/2017/03/write-hololens-apps-on-macos-linux-3.html)

This repository makes use of files from the [unity_remote_build] (https://github.com/psfblair/unity_remote_build) and [unity_fsharp](https://github.com/psfblair/unity_fsharp) repositories.
