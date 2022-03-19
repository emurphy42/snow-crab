using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SnowCrab
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            Helper.Events.GameLoop.GameLaunched += (s, e) => GetContentPatcherAPI();

            // a = WarpedEventArgs
            Helper.Events.Player.Warped += (e, a) => SpawnSnowCrabs(a.NewLocation.NameOrUniqueName);

            /*
            C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\Microsoft.Common.CurrentVersion.targets(2301,5): warning MSB3277:
                Found conflicts between different versions of "Microsoft.Win32.Registry" that could not be resolved.
                There was a conflict between "Microsoft.Win32.Registry, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" and "Microsoft.Win32.Registry, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a".
                    "Microsoft.Win32.Registry, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" was chosen because it was primary and "Microsoft.Win32.Registry, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" was not.
                    References which depend on "Microsoft.Win32.Registry, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" [C:\Users\emurp\.nuget\packages\microsoft.win32.registry\4.3.0\ref\netstandard1.3\Microsoft.Win32.Registry.dll].
                        C:\Users\emurp\.nuget\packages\microsoft.win32.registry\4.3.0\ref\netstandard1.3\Microsoft.Win32.Registry.dll
                          Project file item includes which caused reference "C:\Users\emurp\.nuget\packages\microsoft.win32.registry\4.3.0\ref\netstandard1.3\Microsoft.Win32.Registry.dll".
                            C:\Users\emurp\.nuget\packages\microsoft.win32.registry\4.3.0\ref\netstandard1.3\Microsoft.Win32.Registry.dll
                    References which depend on "Microsoft.Win32.Registry, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" [].
                        C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\StardewModdingAPI.dll
                          Project file item includes which caused reference "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\StardewModdingAPI.dll".
                            StardewModdingAPI
            */

            // Future improvement: chance of daily quests to kill some snow crabs
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Connect to Content Patcher API.</summary>
        private void GetContentPatcherAPI()
        {
            var apiContentPatcher = this.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
            apiContentPatcher.RegisterToken(this.ModManifest, "SnowCrab", () =>
            {
                return new[] { "SnowCrab" };
            });
        }

        /// <summary>Check whether player's location already has another player there.</summary>
        /// <param name="locationName">Name of the new location.</param>
        private bool MultipleFarmersAtLocation(string locationName)
        {
            var numberFarmersAtLocation = 0;
            foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
            {
                if (onlineFarmer.currentLocation.NameOrUniqueName == locationName)
                {
                    ++numberFarmersAtLocation;
                    if (numberFarmersAtLocation > 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Check which mine level (if any) player moved to.</summary>
        /// <param name="locationName">Name of the new location.</param>
        private int GetMineLevel(string locationName)
        {
            if (!locationName.StartsWith("UndergroundMine"))
            {
                return 0;
            }

            var levelNumber = 0;
            int.TryParse(locationName.Replace("UndergroundMine", ""), out levelNumber);
            return levelNumber;
        }

        /// <summary>Check whether player moved to a frozen level with monsters.</summary>
        /// <param name="locationName">Name of the new location.</param>
        private bool IsFrozenLevelWithMonsters(string locationName)
        {
            // Dangerous mines don't have frozen levels
            if (Game1.netWorldState.Value.MinesDifficulty > 0)
            {
                return false;
            }

            // Dungeon levels aren't frozen
            var where = (StardewValley.Locations.MineShaft)Game1.currentLocation;
            if (where.getMineArea() == StardewValley.Locations.MineShaft.quarryMineShaft)
            {
                return false;
            }

            // Frozen levels are 41 through 79
            var levelNumber = GetMineLevel(locationName);
            if (levelNumber < 41 || levelNumber > 79)
            {
                return false;
            }

            // Levels 50, 60, 70 don't have monsters
            if (levelNumber % 10 == 0)
            {
                return false;
            }

            // Yes, it's a frozen level with monsters
            return true;
        }

        /// <summary>Generate a snow crab.</summary>
        /// <param name="pos">Position of the snow crab.</param>
        /// <param name="mineRandom">Random number generator.</param>
        private StardewValley.Monsters.Monster GetNewSnowCrab(Vector2 pos, Random mineRandom)
        {
            // Initializing using just RockCrab() and then setting location caused a crash (Object reference not set to instance of an object)
            // Passing name to RockCrab() caused an error (expects a corresponding asset), this was pre-Content Patcher but not important to analyze now
            var newMonster = new StardewValley.Monsters.RockCrab(pos);

            newMonster.Name = "Snow Crab";

            newMonster.Health = this.Config.SnowCrabHealth;
            newMonster.DamageToFarmer = this.Config.SnowCrabDamageToFarmer;
            // Possible future improvement: adjust base resilience, default 2
            newMonster.ExperienceGained = this.Config.SnowCrabExperienceGained;

            var objectsList = this.Config.SnowCrabObjectsToDrop;
            var objectsSplit = objectsList.Split(' ');
            newMonster.objectsToDrop.Clear();
            for (int i = 0; i < objectsSplit.Length; i += 2)
            {
                if (mineRandom.NextDouble() < Convert.ToDouble(objectsSplit[i + 1]))
                {
                    newMonster.objectsToDrop.Add(Convert.ToInt32(objectsSplit[i]));
                }
            }

            newMonster.Sprite = new AnimatedSprite("SnowCrab");

            return newMonster;
        }

        /// <summary>Replace some monsters with snow crabs if appropriate.</summary>
        /// <param name="locationName">Name of the new location.</param>
        private void SpawnSnowCrabs(string locationName)
        {
            // If another player is here already, then replacement was already triggered when they entered this location
            if (MultipleFarmersAtLocation(locationName))
            {
                return;
            }

            // Replacement only applies to frozen levels with monsters
            if (!IsFrozenLevelWithMonsters(locationName))
            {
                return;
            }

            // Replace some already-spawned monsters with snow crabs
            // Default percentage is interpolated from approximate percentage of monsters on rock/lava levels that are rock/lava crabs
            // Buffs that increase/decrease monsters have already been accounted for
            var where = Game1.currentLocation;
            var mineRandom = new Random();
            var monstersToReplace = new Netcode.NetCollection<NPC>();
            Type oldNPCType;
            int x;
            int y;
            Vector2 pos;
            StardewValley.Monsters.Monster newMonster;

            // Select monsters to replace
            // Replacement occurs later because it alters the contents of getCharacters(), which is what this loop is looping through
            // Possible future improvement: make a copy of the list, loop through copy and do replacements immediately
            foreach (var oldNPC in where.getCharacters())
            {
                oldNPCType = oldNPC.GetType();

                // Is it a monster?
                if (!oldNPCType.IsSubclassOf(typeof(StardewValley.Monsters.Monster))) {
                    continue;
                }

                // Have any snow crabs already spawned here? (player left and came back, and level hasn't reset yet)
                // Possible future improvement: directly check for this situation
                //   (current method fails to detect cases where the mod happened not to replace any monsters)
                // IsSubClassOf() is supposed to also recognize same type, but doesn't for some reason
                if (oldNPCType == typeof(StardewValley.Monsters.RockCrab))
                {
                    return;
                }

                // Never replace ghosts (whose numbers are limited already)
                //   or skeletons (about 75% of monsters on levels 71-79 are skeletons, other 25% have similar distribution to 41-69)
                if (
                    oldNPCType == typeof(StardewValley.Monsters.Ghost)
                    || oldNPCType == typeof(StardewValley.Monsters.Skeleton)
                )
                {
                    continue;
                }

                // Any other monster has random chance to be replaced
                if (mineRandom.NextDouble() < this.Config.SnowCrabChance)
                {
                    monstersToReplace.Add(oldNPC);
                }
            }

            // Replace selected monsters
            foreach (var oldNPC in monstersToReplace)
            {
                // Get location
                x = oldNPC.getTileX();
                y = oldNPC.getTileY();
                pos = new Vector2(x, y) * Game1.tileSize;

                // Remove existing monster
                where.characters.Remove(oldNPC);

                // Spawn snow crab in same location
                newMonster = GetNewSnowCrab(pos, mineRandom);
                where.characters.Add(newMonster);

                // Optionally output to console each time something was changed
                if (this.Config.SnowCrabDebugOutput)
                {
                    this.Monitor.Log($"[Snow Crab] Replaced {oldNPC.Name} with Snow Crab at ({x}, {y})", LogLevel.Debug);
                }
            }
        }
    }
}
