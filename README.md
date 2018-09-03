# Detrapezoidator

Goal of the project: compensate trapezoidal effect of a projector for Unity 2018.2+ projects.

1. How To ?

Just download or clone the project and create a new Unity 3D project. If you want to use it as is, import "CalibStarter.unitypackage" into your project.
In the Calibrator GameObject editor, insert the dimension of the board you are going to project on in meter and press "Setup Screen". It will automatically create all you need.
You can now setup you game into the Scene Camera GameObject.

2. Warnings 

This calibration tool will compensate the trapezoidal effect and the inclination of the projector but if you want it to be correct, the projector must be centered, at the same left and right distance of your board.
Otherwise, the trapezoidal effect won't be corrected equally on both sides.

3. Start of a scene

When your game is configured and you start the scene, first thing you need to do is to click on the 4 corners of your board in order for the calibrating tool to know the edges.
Once it's done, the Camera setup should be fine and that's all.

4. To do 
 - create save system to reload configuration at start
 - find a way to correct uncentered projectors
 - manage a calibration scene and all the "don't destroy" to send to the game scene