# Introduction

A decent traffic system prototype in Unity that I had been thinking about for a long time.
Vehicle behaviour, signal systems, waypoint systems, all written from scratch. Vehicles are quite plain at the moment, with them travelling from waypoint to waypoint and obeying signals of course. They also get reset on stalling for X seconds, but that's it. The vehicles can't go around other vehicles or obstacles right now. 
Technically enabling Auto Repath and navigation quality on the navmesh agents on each vehicle will bring up some results, but at this point they'll open yet another rabbit hole. So that's a project idea for V2 may be! :D

NOTE: This turned out to be a bit of a rabbit hole itself and with the full-time work, personal commitments...and gaming, it got stretched in being almost a year long!

# General Documentation

## Waypoint System

The waypoint system simply manages sets of locations across the map.
- Waypoint class represents 1 single transform on the map.
- PathsManager class is kept in the scene and holds a list of all the paths. 1 path is a sequence of waypoints on which a vehicle will run.
- WaypointManager class is attached to each vehicle. It manages the waypoints path that has been assigned to the vehicle by passing the next waypoint destination, initial position, etc.


## Vehicles

Vehicles module is simply what runs a vehicle on the path.
- Vehicle class is a single responsibility class that has the Navmesh Agent and the Waypoint Manager references and just starts, stops and runs the vehicle to the from destination to destination.
- VehicleCollisionDetection & VehicleTriggerDetection classes do similar, but not the same things. Signals and vehicle proximities are detected through Triggers, meanwhile you banging into another vehicle is detected by a Collision. They just pass different events to the VehicleController.
- VehicleColorVariation is a simple class that would assign a random color to the vehicle's renderer whenever required.
- VehicleConfiguration class holds some basic values regarding a vehicle that would set it's speed, turning angles and some Navmesh Agent properties.
- VehicleStallBehaviour is triggered whenever a vehicle stops. X seconds without starting again and a self destruct is called, which just resets the vehicle back into the object pool.
- VehicleController class is the main manager class that references all Vehicle related classes along with trigger and collision detection and drives the vehicle.
- VehicleGenerator class creates an object pool and keeps dequeuing and enqueuing vehicles from/to the pool.


## Signal System

The signal system runs the signals across the map. It's partially decentralized where each intersection might have multiple signals that run in sync to one another, but the same is not true for every signal on the entire map.
- SignalIndicator is a trigger collider that interacts with the vehicle's trigger detector and passes information regarding the state of the signal.
- SignalManager is a group of signals at any given intersection on the road. A signal manager cycles through all the states(red, yellow & green) and directions(forward, right & left) with help of a TimeBox. 1 element of a timebox is 1 cycle of all the signals together in sync under the SignalManager.
- SignalMVC submodule manages everything within a single signal from the state of a single light(red, yellow, green) to switching between lights and assigning directions. All this along with obviously displaying stuff on the signal object.


## Editor Tools

###Path Creator

The Path Creator tool lets you add each individual waypoint to create a sequence and make a Path. It then pushes that Path onto the list maintained by the PathsManager within the scene.

###Traffic Signal Creator

This tool helps create signals timebox on a synced SignalManager in a little bit easier way. With this tool you can add signals to the SignalManager along with their indicator triggers. You can then go through the Timebox cycle and assign states and directions to each signal within a timebox.
