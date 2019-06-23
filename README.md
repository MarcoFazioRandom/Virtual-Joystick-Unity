# Virtual-Joystick-Unity
A simple and complete virtual joystick for touchscreens, for both 2D and 3D games, with useful options.

<img src="https://raw.githubusercontent.com/MarcoFazioRandom/Virtual-Joystick-Unity/master/preview_1.png?token=AMG4MS3WH3Y73PPEWJ4BB6C5B5QIO" width="800">

<img src="https://raw.githubusercontent.com/MarcoFazioRandom/Virtual-Joystick-Unity/master/preview_2.png?token=AMG4MSYRLDRXLJ4WESTIXM25B5QNY" width="300">

### OPTIONS:  

- Joystick mode: 
	- Fixed: The joystick doesn't move. 
	- Dynamic: Every time the joystick area is pressed, the joystick position is set on the touched position. 
	- Following: If the finger moves outside the joystick background, the joystick follows it.  

- Vector mode: 
	- Real: return a vector with a lenght beetween 0 and 1; useful for implementing different velocity or acceleration.  
	- Normalized: return a normalized vector. 

- Directions: The number of directions, e.g. a D-pad is joystick with 4 directions, keep 0 for a free joystick.  
- Simmetry Angle: the angle of simmetry of the directions.  

- Dead zone: If the handle is inside this range, in proportion to the background size, the output is zero.

- Clamp zone: The max distance the handle can reach, in proportion to the background size. 
