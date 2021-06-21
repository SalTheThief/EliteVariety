# Elite Variety
Adds 6 new elite types! Supports [Aspect Abilities](https://thunderstore.io/package/TheMysticSword/AspectAbilities/) for on-use effects.  

---
###### Ironclad - spawns with full barrier that doesn't decay. Roots on hit. On use: gain temporary armor.
![](https://i.imgur.com/xse1DZd.png)

---
###### Invigorating - carries a warbanner that buffs nearby allies. Temporarily changes Warbanner buff to Frenzy on hit. On use: increase banner radius.
![](https://i.imgur.com/HEHQOXf.png)

---
###### Pillaging - steals gold from nearby allies. Steals gold from the victim on hit. On use: spend all of your gold and gain a random item. The more gold spent, the higher chance of getting a rarer item.
![](https://i.imgur.com/gfWxSc5.png)

---
###### Arenaceous - always surrounded by a sandstorm that deals damage to nearby enemies. Reduces visibility on hit. On use: dash forward, knocking nearby enemies up. Armor increased during the dash.
![](https://i.imgur.com/5Le5nhZ.png)

---
###### Tinkering - spawns up to 3 Tinkerer's Drones every few seconds that become stronger with the amount of Item Scrap in the inventory. Steal the victim's Item Scrap on hit. On use: heal all ally drones.
![](https://i.imgur.com/Y2TIUEJ.png)

---
###### ERYTHRITE (loop exclusive) - targets looking at this elite will take more damage the longer they look at it. Impales on hit, causing to periodically take high damage over 60 seconds. On use: teleport to the victim and deal damage after a short period of time.
![](https://i.imgur.com/RXR2ab1.png)

---
### Changelog:
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
  
(Previous changelogs can be found [here](https://github.com/TheMysticSword/EliteVariety/blob/main/CHANGELOG.md))
