#### 1.0.2:
* Fixed Tinkerer's Drones being able to use the Tinkering on-use ability if they inherit the aspect equipment
#### 1.0.1:
* Arenaceous:
	* The sandstorm now deals a fixed amount of damage that scales with level instead of dealing a percentage of the owner's damage
	* The sandstorm no longer rotates with its owner
		* This change mainly affects Wisps and Jellyfish
	* On-use dash ability is no longer affected by gravity if the owner is a flying character (Wisps, Jellyfish, etc.)
	* On-use dash now has slight direction control
* Pillaging:
	* Aspect ability now spends ALL of your gold and cannot be activated unless you have $25 (scaling over time)
	* Pillaging enemies will no longer drop less gold after spending gold with their aspect ability
* Tinkering:
	* Now permanently steal Scrap on hit with 100% chance instead of having a chance to steal an item and converting it into Scrap
	* Tinkerer's Drones can no longer become Tinkering
		* If a Tinkerer's Drone becomes Tinkering anyway, it will not spawn more Tinkerer's Drones
	* Tinkerer's Drone attack is no longer affected by the global 0.5s skill cooldown cap
		* This change mainly affects Tinkering players with a lot of Item Scrap, allowing their drones to bypass the cooldown cap and fire rapidly at high attack speed
	* Reduced Tinkerer's Drone laser hitbox radius
* Erythrite:
	* Changed stare debuff functionality: continuously looking at the elite will build up stacks of a debuff that increases damage against you by 10% per stack; stacks will slowly wear off when not looking at the elite
	* Changed on-use ability: now teleports to the target after a short period of time and deals damage in a small radius
	* Buffed Impaled DoT damage
	* Made the stare effect collision consistent at all distances
* Fixed Tinkerer's Drone not losing health after its owner dies if the owner was an Engineer Turret
* Fixed Pillaging on-use ability's odds of giving better items increasing over time
* Fixed Pillaging enemies being able to purchase items that are banned from monsters
* Fixed Arenaceous sandstorm collider blocking pings