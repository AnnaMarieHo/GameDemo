# GameDemo
3D Game Engine Design Final Project

## Project Description (Synopsis)
This CSC 316 term project was intended to be a basic 3D remake of the game, Realm of the Mad God. Like the game’s initial game release, A player with the ability to shoot projectiles has been included, as well as multiple enemies that chase a player. The player must defeat enemies to survive and complete the game successfully.

## Player
The player’s objective is to survive enemy attacks while retaliating with attacks of their own. To avoid losing HP, the player will need to destroy as many enemies as possible, as well as attempt to outrun them. The player has the ability to move forward, left, and right. In addition to this movement range, the player can fire single projectiles, as well as multiple projectiles. When multiple projectiles are shot, they have no set direction and will destroy any enemy in the general vicinity.

## Enemy
Enemies fire single projectiles whenever the player comes within a certain range. The enemies will exist in a default patrol state and wander in the game environment. The attack state will only be triggered if the player comes within the enemy's attack krange. When the enemy has detected the player, it will pursue and attack the player until the enemy is either defeated or outrun.

## Controls
The player will use the **WASD** keys to navigate the game. **W** will move the player forward, **A** will move the player left, and **D** will move the player right. To shoot a stream of projectiles, the player may hold down the **LMB**, or use a single click to shoot one projectile. The player can switch to shoot multiple projectiles with no set direction after the **SHIFT** key is pressed.




## Enemy Behavior
The enemy class is given 3 different behaviors: **Freeze**, **Random**, and **Attack**.
### Random Behavior: 
An enemy instance is generated in a random position. The random behavior is achieved by adjusting the enemies’ rotation randomly between the range (-3, 3) while the enemies’ positions are translated by using the enemy’s current rotation and velocity. This makes it possible for the enemy to move in minor random directions. 

###	Freeze Behavior: 
Freeze behavior occurs when an enemy comes close to the player and freezes to attack. It is achieved by using a switch statement so that no motion will be accessed.

### Attack Behavior:
The enemy attack behavior begins if a player comes within a certain distance of the enemy. When the player has come close enough, an enemy’s rotation is set to point at the player, and its speed is increased therefore moving the enemy in the player’s direction. This speed increase allows the enemy to continuously orient to the player’s position and stay within a certain distance of a player for a short period of time. Ultimately, this allows the enemies to “chase” the player. 

## Enemy Collision
Because enemies follow the player, they start to converge at one point and therefore need to collide with each other. 
Each enemy has a bounding sphere. The bounding sphere will take two arguments, the enemy instance’s position, and radius. When an enemy instance is generated, two foreach loops in the main class will cycle through a list of all enemies and determine if the current enemy bounding sphere intersects with the other enemy’s bounding sphere. If they do collide, then the enemies will be prevented from intersecting with each other. 

## Enemy Attack
Whenever an enemy enters attack behavior, a method called “Shoot” will be called. This method will create projectile instances and add them to a list of projectiles. A loop will then cycle through these projectiles and give each a forward motion (by calling Projectile.Update), as well as remove them when they hit the ground. Because the shooting method belongs to the enemy class, each time an enemy instance is created, an enemy will have the ability to shoot. This allows all instances of an enemy to shoot and makes it possible for multiple to shoot at the player.

## Player Behavior
The player will move using the **WASD** keys and shoot with **LMB**. When the player presses **A** and **D**, the players rotation will be increased. Therefore, when the player moves forward with **W**, they will be pushed in the direction of their current rotation, by increasing the current orientation by velocity * speed. 

## Player Attack
The player’s mouse state will be captured to determine whether a projectile instance should be created. If the player is shooting, a projectile instance will be added to an array in the main class and its velocity and position will be updated to give it motion. 

## Player – Enemy Projectile Collision
The collision between the player and enemy projectiles needs major improvement but is currently functional. The player and enemy projectile collision is achieved by cycling through each enemy instance, and further cycling through each enemies’ list of projectile instances. The Distance between the enemy’s projectile and the player is then calculated. If the distance is less than the player’s radius, it will save life. Player lives will only be decreased if the projectile intersects a particular short distance within the player radius. This causes many problems as a specific distance cannot always be detected. Likewise, to calculate this distance, the player may not go higher than a certain height and the enemies must freeze within a particular distance. Due to these problems, I hope to take time and decrease player lives more accurately by using bounding spheres and gametime. This might allow detection of projectiles that intersect with the player, but not allow the lives to decrease for more than one frame of the game. 

## Player Lives
A player’s life is decreased each time a projectile collides with the player. The distance between each enemy instance’s projectiles and the player is calculated. If the distance is less than the enemy’s radius, a life will be removed.
