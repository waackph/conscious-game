# conscious-game
This is a small adventure game project focussed on prototyping.

## Next Steps

- Implement new Game Elements by Implementing first room
	<br> 
    (Depressed mode -> use shower -> half depressed mode -> tar morphs to coffee -> drink coffee -> ready to go)
    - Mental State Indicator
    - Altering/morphing World
    - Stream of Consciousness (UI Element with Sentences -> reacts on being near specific things in level)
- Define Art Process by drawing sprites for first room
    - Draw background
    - Draw items
    - Draw character
    - Animate character

## Keep in mind while coding:
- Cohesion - code/function does only one task
- DRY - Dont Repeat Yourself
- Coupling - principle of "separation of concerns" (one object doesn't directly change or modify the state or behavior of another object.)
	<br>
    -> You may use relation diagrams and intermediate classes to decouple
	<br>
	(good) loose coupling: Objects that are independent from one another and do not directly modify the state of other objects
	<br>
	(bad) tight coupling: Objects that rely on other objects and can modify the states of other objects are said to be tightly coupled
	<br>
	=> Encapsulation helps (principle of hiding information that are not important for rest of program/code)
- Helpful concepts:
    - Component based entities
    - Event/Message system
	<br>

!=> "strive for low coupling and high cohesion"

